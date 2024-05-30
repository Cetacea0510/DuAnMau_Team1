using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue;
    // Định nghĩa một sự kiện để thông báo rằng coin đã được nhân vật chạm vào
    public delegate void CoinCollected();
    public static event CoinCollected OnCoinCollected;

    // Hàm được gọi khi một Collider khác chạm vào Collider của coin
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có phải là nhân vật không
        if (other.CompareTag("Player"))
        {
            // Gọi sự kiện thông báo rằng coin đã được nhân vật chạm vào
            if (OnCoinCollected != null)
                OnCoinCollected();

            // Sau khi coin được nhân vật chạm vào, ta có thể thực hiện các hành động khác, như làm mất coin đi, hoặc tăng điểm số, vv.
            // Ví dụ: Ta có thể làm coin biến mất
            Destroy(gameObject);
        }
    }
}
