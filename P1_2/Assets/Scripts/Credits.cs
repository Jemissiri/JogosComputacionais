using UnityEngine;

public class Credits : MonoBehaviour
{
    void OnGUI()
    {
        float boxWidth = 400;
        float boxHeight = 340;
        float boxY = Screen.height - boxHeight - 10f;

        GUI.skin.label.fontSize = 13;

        GUI.Label(new Rect(10, boxY, boxWidth, boxHeight),
            "Road texture: https://pt.pinterest.com/pin/6685099438055248/\n\n" +
            "Grass texture:\nhttps://www.freepik.com/free-vector/seamless-green-grass-pattern_13187581.htm#fromView=keyword&page=1&position=1&uuid=470d536d-ffd9-4451-a048-571fb14cbeb2&query=Grass+texture\n\n" +
            "Balls texture: https://www.oddballs.co.uk/cdn/shop/files/Oddballs-Bouncing-Ball---65mm_pic1_odd_n.jpg?v=1695819130\n\n" +
            "Pillars texture:\nhttps://www.freepik.com/free-photo/background-made-from-bricks_10980125.htm#fromView=keyword&page=1&position=0&uuid=e26910bb-4e29-4d96-9650-49b2fa30ca52&query=Bricks+texture\n\n" +
            "The vehicles were developed by Unity\nfor the Junior Programmer learning pathway.");
    }
}
