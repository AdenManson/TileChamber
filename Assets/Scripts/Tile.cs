using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private BoardManager boardManager;
    private SoundManager soundManager;
    private ScoreManager scoreManager;
    private TileAnimator anim;

    public Sprite[] sprites;

    public Color blueCol;
    public Color redCol;
    public Color greenCol;
    public Color yellowCol;
    private Color overlayCol = new Color(1f, 1f, 1f, 1f);
    private Color currentCol;

    [System.NonSerialized]
    public int gridPosX, gridPosY;
    private Vector3 tileScale;

    [System.NonSerialized]
    public float moveAmount; // used by BoardManager to move tile down

    Tile[] neighbours;

    private Renderer ren;
    private SpriteRenderer sr;
    enum TileColour
    {
        BLUE,
        RED,
        GREEN,
        YELLOW
    };

    enum TileType
    {
        NORMAL,
        DYNAMITE,
        COLOUR
    };

    TileColour tileColour;
    TileType tileType;

    private void Awake()
    {
        boardManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardManager>();
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        scoreManager = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreManager>();

        anim = GetComponent<TileAnimator>();
        tileScale      = new Vector3(boardManager.getTileScale(), boardManager.getTileScale());
    }
    void Start()
    {
        gameObject.transform.localScale = tileScale;

        ren = GetComponent<Renderer>();
        sr = GetComponent<SpriteRenderer>();

        // Pick random tile and assign colour
        tileColour = (TileColour)Random.Range(0, System.Enum.GetValues(typeof(TileColour)).Length);

        // Change to corresponding colour
        if (tileColour == TileColour.BLUE)   currentCol = blueCol;
        if (tileColour == TileColour.RED)    currentCol = redCol;
        if (tileColour == TileColour.GREEN)  currentCol = greenCol;
        if (tileColour == TileColour.YELLOW) currentCol = yellowCol;
        ren.material.color = currentCol;

        // Pick random tile type

        int tileTypeIndex = Random.Range(0, 80);
        switch (tileTypeIndex)
        {
            case 0:
                sr.sprite = sprites[2];
                tileType = TileType.DYNAMITE;
                break;
            case 1:
                sr.sprite = sprites[3];
                tileType = TileType.COLOUR;
                break;
            default:
                tileType = TileType.NORMAL;

                // Pick random texture
                int spriteID = Random.Range(0, 1);
                sr.sprite = sprites[spriteID];
                break;
        }

        // Flip texture random direction
        if (tileType == TileType.NORMAL)
        {
            int randomDir = Random.Range(0, 3);
            float dir = 0f;
            switch (randomDir)
            {
                case 0:
                    dir = 0f;
                    break;
                case 1:
                    dir = 90f;
                    break;
                case 2:
                    dir = 180f;
                    break;
                case 3:
                    dir = 270f;
                    break;
            }
            transform.rotation = Quaternion.Euler(0, 0, dir);
        }
    }

    private void OnMouseEnter()
    {
        //ren.material.color = CombineColors(currentCol, overlayCol);
    }
    private void OnMouseExit()
    {
        ren.material.color = currentCol;
    }

    // Method from Bunny83: http://answers.unity.com/answers/725909/view.html
    public static Color CombineColors(params Color[] aColors)
    {
        Color result = new Color(0, 0, 0, 0);
        foreach (Color c in aColors)
        {
            result += c;
        }
        result /= aColors.Length;
        return result;
    }
    private void OnMouseDown()
    {
        gameObject.transform.localScale = tileScale - new Vector3(0.03f, 0.03f);

        // Check if tile is at target destination (finished animation)
        if(boardManager.canDestroy)
        {
            // Check if multi destroy is active
            if (boardManager.multiDestroy)
                boardManager.destroyRows(this);
            else
                if (tileType == TileType.NORMAL) // Normal deletion
                    deleteTiles(this);
                if (tileType == TileType.DYNAMITE) // Dynamite deletion
                    boardManager.destroyArea(this, true);
                if (tileType == TileType.COLOUR) // Colour deletion
                    boardManager.destroyColours(this, true);
        }

        soundManager.playSound("tile_tap");
    }

    public void deleteTiles(Tile lastTile)
    {
        // Get Neighbours
        neighbours = boardManager.getNeighbours(this);

        // Loop over neighbours
        foreach (Tile tile in neighbours)
        {
            if (tile != null)
            {
                if (tile.tileColour == lastTile.tileColour)
                {
                    // Check if tile has already been added to remove list
                    if (!boardManager.isTileInRemoveList(tile))
                    {
                        boardManager.addTileToRemoveList(tile);

                        // Check tile type
                        if (tile.tileType == TileType.DYNAMITE)
                            boardManager.destroyArea(tile, false);
                        else if (tile.tileType == TileType.COLOUR)
                            boardManager.destroyColours(tile, false);

                        // Recursive step
                        tile.deleteTiles(this);
                    }
                }
            }
        }
    }

    private void OnMouseUp()
    {
        gameObject.transform.localScale = tileScale;
        boardManager.removeTiles();
    }

    public string getTileType()
    {
        return tileType.ToString();
    }

    public string getTileColour()
    {
        return tileColour.ToString();
    }

    public Color getTileMatColour()
    {
        return ren.material.color;
    }

    public void removeTile()
    {
        anim.deleteAnim(this);
    }

    public void moveDown(float y)
    {
        boardManager.canDestroy = false;
        
        anim.tweenTile(this, new Vector3(transform.localPosition.x, transform.localPosition.y - y), 0.3f);
    }

    // Finished falling
    public void setCanDestroy(bool status)
    {
        boardManager.canDestroy = status;
    }

    public void setGridPos(int x, int y)
    {
        gridPosX = x;
        gridPosY = y;
    }
}
