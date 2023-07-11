/***********************************************************/
/**  © 2018 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru/
/**  Подписка на Рatreon: https://www.patreon.com/null_code
/***********************************************************/

using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class BotArea : MonoBehaviour
{
    public BotControl[] bots;
    [Range(0, 1f)] public float movePercent = .5f; // процент вероятности, на передвижение бота
    public float timeMin = 5, timeMax = 10; // мин макс время ожидания до следующего перемещения
    public BoxCollider boxCollider;
    public LayerMask playerMask; // маска игрока
    private float timeout, curTime;
    public bool IsAlert { get; private set; }

    void OnDrawGizmos()
    {
        Gizmos.color = IsAlert ? new Color(1, 0, 0, 0.25f) : new Color(0, 1, 0, 0.25f);
        Gizmos.DrawCube(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z));
    }

    void OnValidate()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    public bool CheckMask(Collider obj)
    {
        if (((1 << obj.gameObject.layer) & playerMask) != 0)
        {
            return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (CheckMask(other))
        {
            IsAlert = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (CheckMask(other))
        {
            IsAlert = false;
        }
    }

    void Start()
    {
        curTime = Random.Range(timeMin, timeMax);

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i].StartBot(this);
            bots[i].FindRandomPoint(boxCollider.bounds);
        }
    }

    public void FindPoint(BotControl bot)
    {
        bot.FindRandomPoint(boxCollider.bounds);
    }

    void Update()
    {
        timeout += Time.deltaTime;

        if (timeout > curTime)
        {
            for (int i = 0; i < bots.Length; i++)
            {
                int j = Random.Range(0, bots.Length);

                if (!bots[i].IsAlert && Random.value > 1f - movePercent && bots[i].agent.velocity.magnitude == 0)
                {
                    bots[i].FindRandomPoint(boxCollider.bounds);
                }
            }

            timeout = 0;
            curTime = Random.Range(timeMin, timeMax);
        }

        for (int i = 0; i < bots.Length; i++)
        {
            bots[i].UpdateBot();
        }
    }
}
