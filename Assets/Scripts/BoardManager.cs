using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private SoundManager soundManager;
    private ScreenShake screenShake;
    private ScoreManager scoreManager;

    int NUM_COLS = 9;
    int NUM_ROWS = 9;
    private Tile[,] board;

    private float tileScale = 0.2f;
    private float tileUnit;

    [SerializeField]
    private Tile tilePrefab;

    private List<Tile> tilesToRemove = new List<Tile>();

    [SerializeField]
    private ParticleSystem breakParticlesPrefab;

    [System.NonSerialized]
    public bool multiDestroy = false;

    [System.NonSerialized]
    public bool canDestroy = true; // determines if tile can be clicked on
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        screenShake  = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ScreenShake>();
        scoreManager = GameObject.FindGameObjectWithTag("Score").GetComponent<ScoreManager>();

        initBoard();
    }
    private void Update()
    {

    }

    void initBoard()
    {
        // Init Board Array
        board = new Tile[NUM_COLS, NUM_ROWS];
        tileUnit = tileScale * 2.55f;

        float startX = (-(NUM_COLS - 1) / 2f) * tileUnit;
        float startY = (-(NUM_ROWS - 1) / 2f) * tileUnit;

        // Spawn Tiles
        for (int y = 0; y < NUM_ROWS; y++)
        {
            for (int x = 0; x < NUM_COLS; x++)
            {
                // Instantiate Tile
                spawnTile(x, y, 0);
            }
        }

    }

    // Return a list of neighbours to a tile
    public Tile[] getNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();
        int tileX = tile.gridPosX;
        int tileY = tile.gridPosY;

        // Top
        if (tileY < NUM_ROWS-1)
            neighbours.Add(board[tileX, tileY + 1]);
        // Bottom
        if (tileY > 0)
            neighbours.Add(board[tileX, tileY - 1]);
        // Right
        if (tileX < NUM_COLS-1)
            neighbours.Add(board[tileX + 1, tileY]);
        // Left
        if (tileX > 0)
            neighbours.Add(board[tileX - 1, tileY]);


        return neighbours.ToArray();
    }

    public void addTileToRemoveList(Tile tile)
    {
        tilesToRemove.Add(tile);
    }

    public bool isTileInRemoveList(Tile tile)
    {
        return tilesToRemove.Contains(tile);
    }

    public bool isTileRemoveListEmpty()
    {
        return tilesToRemove.Count == 0;
    }
    public void removeTiles()
    {
        if (tilesToRemove.Count == 0) return;

        foreach (Tile tile in tilesToRemove)
        {
            // Remove tile from board
            board[tile.gridPosX, tile.gridPosY] = null;
            tile.removeTile();

            // Particle FX
            Vector3 tilePos = tile.gameObject.transform.localPosition;
            ParticleSystem breakParticles = Instantiate(breakParticlesPrefab, new Vector3(tilePos.x, tilePos.y, -2f), Quaternion.identity);
            breakParticles.startColor = tile.getTileMatColour();

            // Add points to score
            scoreManager.addScore(1);
        }
        // Clear list
        tilesToRemove.Clear();

        // Update tile positions
        updateTilePositions();

        // FX
        soundManager.playSound("tile_break");
        screenShake.TriggerShake(0.25f, 0.05f);
    }

    private void replaceTiles()
    {
        // Loop through tiles bottom up, replace each empty space with a tile
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if(board[x,y] == null)
                    spawnTile(x, y, 10);
            }
        }
    }

    private void spawnTile(int gridPosX, int gridPosY, int offY)
    {
        float startX = (-(NUM_COLS - 1) / 2f) * tileUnit;
        float startY = (-(NUM_ROWS - 1) / 2f) * tileUnit;

        Tile tile = Instantiate(tilePrefab, new Vector3(startX, startY, 0), Quaternion.identity);
        Vector3 newPos = new Vector3(startX + (gridPosX * tileUnit), offY, 0);
        float tarY = offY - (startY + (gridPosY * tileUnit));

        if(offY == 0)
            newPos = new Vector3(startX + (gridPosX * tileUnit), (startY + (gridPosY * tileUnit)), 0);

        tile.transform.position = newPos;
        tile.setGridPos(gridPosX, gridPosY);

        if(offY != 0)
            tile.moveDown(tarY);

        board[gridPosX, gridPosY] = tile;
    }

    public void updateTilePositions()
    {
        List<Tile> movedTiles = new List<Tile>();

        // Logically move tiles down
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                Tile tile = board[x, y];
                if (tile != null)
                {
                    if (y > 0 && y < board.GetLength(1))
                    {
                        int yy = y;
                        while (yy > 0)
                        {
                            if (board[x, yy - 1] == null)
                            {
                                board[x, yy - 1] = tile;
                                board[x, yy] = null;
                                tile.gridPosY--;
                                tile.moveAmount += tileUnit;

                                if (!movedTiles.Contains(tile))
                                {
                                    movedTiles.Add(tile);
                                }
                            }
                            yy--;
                        }

                    }
                }
            }
        }

        // Physically move tiles down (animate)
        foreach (Tile movedTile in movedTiles)
        {
            movedTile.moveDown(movedTile.moveAmount);
            movedTile.moveAmount = 0;
        }

        // Replace tiles after all tiles have fallen into new place
        replaceTiles();
    }
    public void destroyRows(Tile originTile) {
        float tileY = originTile.gridPosY;

        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                Tile tile = board[x, y];
                if (tile != null)
                {
                    if ((y > tileY - 2 && y < tileY + 2) && canDestroy)
                    {
                        // Check tile type
                        if (tile.getTileType().Equals("DYNAMITE"))
                            destroyArea(tile, false);
                        if (tile.getTileType().Equals("COLOUR"))
                            destroyColours(tile, false);
                        else
                            addTileToRemoveList(tile);
                    }
                }
            }
        }
        removeTiles();
        multiDestroy = false;
        scoreManager.disableMultiDestroyButton();
        toggleMultiDestroy();

        // FX
        soundManager.playSound("tile_big_break");
        screenShake.TriggerShake(0.35f, 0.08f);
    }

    public void destroyArea(Tile originTile, bool remove)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                Tile tile = board[x, y];
                if (tile != null)
                {
                    if ((isTileInArea(originTile, tile, 3)) && canDestroy)
                    {

                        if (!isTileInRemoveList(tile))
                            addTileToRemoveList(tile);
                    }
                }
            }
        }
        if(remove)
            removeTiles();

        // FX
        soundManager.playSound("tile_big_break");
        screenShake.TriggerShake(0.35f, 0.08f);
    }

    public void destroyColours(Tile originTile, bool remove)
    {
        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                Tile tile = board[x, y];
                if (tile != null)
                {
                    if (tile.getTileColour() == originTile.getTileColour() && canDestroy)
                    {
                        if(!isTileInRemoveList(tile))
                            addTileToRemoveList(tile);
                    }
                }
            }
        }
        if (remove)
            removeTiles();

        // FX
        soundManager.playSound("tile_big_break");
        screenShake.TriggerShake(0.35f, 0.08f);
    }

    private bool isTileInArea(Tile originTile, Tile tile, float rad)
    {
        float cX = originTile.gridPosX;
        float cY = originTile.gridPosY;
        float x = tile.gridPosX;
        float y = tile.gridPosY;

        double dist = Math.Sqrt(Math.Pow((x - cX), 2) + (Math.Pow((y - cY), 2)));
        return (dist <= rad);
    }

    public Tile[,] getBoard()
    {
        return board;
    }

    public float getTileScale()
    {
        return tileScale;
    }

    public void toggleMultiDestroy()
    {
        multiDestroy = !multiDestroy;
    }
}
