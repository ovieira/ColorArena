using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class hexagon_script : MonoBehaviour {

    public Sprite white, red, green, blue;

    private SpriteRenderer _spriterenderer;
    private Color _spritecolor;

    public enum HexagonColor {
        WHITE,
        RED,
        GREEN,
        BLUE
    }

    public HexagonColor currentColor;

    //public GameObject[] adjacent;
    public List<GameObject> adjacent;
    public int count;

    public Vector2 grid_pos;
    public Vector2 screen_pos;
    public GUIStyle _style;
    private bool end = false;
    private Color endColor;
    private bool winning_animation_bool = true;

    // Use this for initialization
    void Start() {
        currentColor = HexagonColor.WHITE;
        //adjacent = new GameObject[6];
        screen_pos = Camera.main.WorldToScreenPoint(transform.position);
        _spriterenderer = GetComponent<SpriteRenderer>();
        _spritecolor = _spriterenderer.color;
    }

    // Update is called once per frame
    void Update() {
        if(!end)
            _spriterenderer.color = Color.Lerp(_spriterenderer.color, getCurrentColor(), Time.deltaTime*5);
        else {
            if (winning_animation_bool) {
                float t = (Mathf.Sin(Time.time)+1)/2;
                _spriterenderer.color = Color.Lerp(getCurrentColor(), endColor, t);
            }
            else
                _spriterenderer.color = Color.Lerp(_spriterenderer.color, endColor, Time.deltaTime);
        }
    }

    private Color getCurrentColor() {
        switch (currentColor) {
            case HexagonColor.WHITE:
                return Color.white;
            case HexagonColor.RED:
                return Color.red;
            case HexagonColor.GREEN:
                return Color.green;
            case HexagonColor.BLUE:
                return Color.blue;
        }
        return Color.white;
    }

    public void setColor(HexagonColor c) {
        captureTile(c);
        //GameObject.FindGameObjectWithTag("GameManager").SendMessage("IncPlayer");
    }

    public void captureTile(HexagonColor c) {
        switch (c) {
            case HexagonColor.RED:
                //GetComponent<SpriteRenderer>().sprite = red;
                currentColor = HexagonColor.RED;
                break;
            case HexagonColor.GREEN:
                //GetComponent<SpriteRenderer>().sprite = green;
                currentColor = HexagonColor.GREEN;
                break;
            case HexagonColor.BLUE:
                //GetComponent<SpriteRenderer>().sprite = blue;
                currentColor = HexagonColor.BLUE;
                break;
            default:
                break;
        }
    }

    private void checkAdjacentTiles(HexagonColor c) {

    }

    public void SetGridPosition(Vector2 pos) {
        grid_pos = pos;
    }

    public Vector2 GetGridPosition() {
        return grid_pos;
    }

    public bool isWhite() {
        return currentColor == HexagonColor.WHITE;
    }

    public bool sameColor(HexagonColor c) {
        return currentColor == c;
    }

    void OnGUI() {
        GUI.Label(new Rect(screen_pos.x - 15, Screen.height - screen_pos.y, 10, 20), grid_pos.ToString(), _style);
    }

    [ContextMenu("WinningTile")]
    public void WinningTile() {
        end = true;
        endColor = Color.white;
        winning_animation_bool = true;
    }

    [ContextMenu("LoosingTile")]
    public void LoosingTile() {
        endColor = Color.black;
        end = true;
        winning_animation_bool = false;
    }

}
