using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int cointsCount;
    [SerializeField] private Text coinsText;
    public BuffReciever buffReciever;
    private List<Item> items;
    public List<Item> Items
    {
        get { return items; } 
    }
    private void Start()
    {
        GameManager.Instance.inventory = this;
        coinsText.text = cointsCount.ToString();
        items = new List<Item>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (GameManager.Instance.coinContainer.ContainsKey(col.gameObject))
        {
            cointsCount++;
            coinsText.text = cointsCount.ToString();
            var coin = GameManager.Instance.coinContainer[col.gameObject];
            coin.StartDestroy();
        }
        if (GameManager.Instance.itemContainer.ContainsKey(col.gameObject))
        {
            var itemComponent = GameManager.Instance.itemContainer[col.gameObject];
            items.Add(itemComponent.Item);
            itemComponent.Destroy(col.gameObject);
        }
    }
}