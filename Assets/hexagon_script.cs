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

    public GameObject[] adjacent;
    public int count;

	// Use this for initialization
	void Start () {
        currentColor = HexagonColor.WHITE;
        adjacent = new GameObject[6];
        AddAdjacentTiles();
	}

    [ContextMenu("Adjacent")]
    private void AddAdjacentTiles() {
        Vector3 p = transform.right;

        for (int i = 0; i < 6; i++) {
            Vector3 new_p = Quaternion.Euler(0, 0, i * 60) * p;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new_p * 0.5f, new_p);
            Debug.DrawRay(transform.position + new_p * 0.5f, new_p, Color.black, 1.0f);
            if (hit.collider != null) {
                adjacent[i] = hit.collider.gameObject;
                count++;
                Debug.Log(hit.collider.name);
            }
        }  
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
