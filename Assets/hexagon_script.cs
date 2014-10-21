using UnityEngine;
using System.Collections;

public class hexagon_script : MonoBehaviour {

    public Sprite white, red, green, blue;

    public enum HexagonColor {
        WHITE,
        RED,
        GREEN,
        BLUE
    }

    public HexagonColor currentColor;

	// Use this for initialization
	void Start () {
        currentColor = HexagonColor.WHITE;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void setColor(HexagonColor c) {
        if (currentColor == HexagonColor.WHITE) {
            switch (c) {
                case HexagonColor.RED:
                    GetComponent<SpriteRenderer>().sprite = red;
                    currentColor = HexagonColor.RED;
                    break;
                case HexagonColor.GREEN:
                    GetComponent<SpriteRenderer>().sprite = green;
                    currentColor = HexagonColor.GREEN;
                    break;
                case HexagonColor.BLUE:
                    GetComponent<SpriteRenderer>().sprite = blue;
                    currentColor = HexagonColor.GREEN;
                    break;
                default:
                    break;
            }
            GameObject.FindGameObjectWithTag("GameManager").SendMessage("IncPlayer");
        }
    }
}
