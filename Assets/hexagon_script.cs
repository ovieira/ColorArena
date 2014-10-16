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
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void setColor(HexagonColor c) {
        switch (c) {
            case HexagonColor.WHITE:
                GetComponent<SpriteRenderer>().sprite = white;
                break;
            case HexagonColor.RED:
                GetComponent<SpriteRenderer>().sprite = red;
                break;
            case HexagonColor.GREEN:
                GetComponent<SpriteRenderer>().sprite = green;
                break;
            case HexagonColor.BLUE:
                GetComponent<SpriteRenderer>().sprite = blue;
                break;
            default:
                break;
        }
    }
}
