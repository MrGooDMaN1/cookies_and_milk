/***********************************************************/
/**  © 2018 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru/
/**  Подписка на Рatreon: https://www.patreon.com/null_code
/***********************************************************/

using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SphereCollider))]
public class BotControl : MonoBehaviour
{
    public Transform[] wayPoints; // вейпоинты, если не указанны, то юнит будет двигаться в рамках зоны (точек должно быть больше одной)
    public float waitTime = 3; // время ожидания, после достижения точки вейпоинта
    public NavMeshAgent agent;
    public float searchRadius = 5; // радиус поиска игрока
    public float maxFollowDistance = 15; // макс дистанция преследования
    public SphereCollider searchCollider;
    public bool IsSearch { get; private set; }
    public bool IsAlert { get; private set; }
    public bool IsWayPoints { get; private set; }
    public BotArea area { get; private set; }
    public Transform target { get; private set; }
    private LayerMask layerMask;
    private int wayIndex;
    private float timeout;
    private bool getWay;

    void OnTriggerEnter(Collider other)
    {
        if (area.CheckMask(other))
        {
            IsSearch = true;
            searchCollider.enabled = false;
        }
    }

    void OnValidate()
    {
        agent = GetComponent<NavMeshAgent>();
        searchCollider = GetComponent<SphereCollider>();
        searchCollider.isTrigger = true;
        searchCollider.enabled = true;
        searchCollider.radius = searchRadius;
    }

    public void StartBot(BotArea botArea)
    {
        area = botArea;
        layerMask = 1 << gameObject.layer | 1 << 2;
        layerMask = ~layerMask;

        if (wayPoints.Length > 1)
        {
            IsWayPoints = true;
        }
    }

    Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3
            (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
            );
    }

    public void FindRandomPoint(Bounds bounds)
    {
        if (wayPoints.Length > 1) return;
        agent.SetDestination(RandomPointInBounds(bounds));
    }

    public void UpdateBot()
    {
        if (IsAlert) Follow();
        if (IsSearch) Search();
        if (IsWayPoints) WayPoints();
    }

    void WayPoints()
    {
        agent.SetDestination(wayPoints[wayIndex].position);

        if (agent.velocity.magnitude == 0.0f && Vector3.Distance(wayPoints[wayIndex].position, transform.position) > agent.radius)
        {
            timeout += Time.deltaTime;
            if (timeout > waitTime)
            {
                timeout = 0;
                if (wayIndex < wayPoints.Length - 1) wayIndex++; else wayIndex = 0;
            }
            
        }
    }

    void Follow()
    {
        Debug.DrawLine(transform.position, target.position, Color.cyan);

        agent.SetDestination(target.position);

        if (Vector3.Distance(transform.position, target.position) > maxFollowDistance)
        {
            IsAlert = false;
            searchCollider.enabled = true;

            if (wayPoints.Length > 1)
            {
                IsSearch = true;
                IsWayPoints = true;
            }
            else
            {
                IsSearch = area.IsAlert;
                IsWayPoints = false;
                area.FindPoint(this);
            }
        }
    }

    void Search()
    {
        target = GetNearTarget(Physics.OverlapSphere(transform.position, searchRadius, area.playerMask));

        if (target == null)
        {
            IsSearch = false;
            searchCollider.enabled = true;
            return;
        }

        RaycastHit hit;
        if (Physics.Linecast(transform.position, target.position, out hit, layerMask))
        {
            if (area.CheckMask(hit.collider))
            {
                IsSearch = false;
                IsAlert = true;
                IsWayPoints = false;
            }
        }
    }

    Transform GetNearTarget(Collider[] collider)
    {
        Transform current = null;
        float dist = Mathf.Infinity;

        foreach (Collider coll in collider)
        {
            float currentDist = Vector3.Distance(coll.transform.position, transform.position);

            if (currentDist < dist)
            {
                current = coll.transform;
                dist = currentDist;
            }
        }

        return current;
    }
}
