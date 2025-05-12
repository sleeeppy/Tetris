using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public GameObject GameOverUI;

    private float lastLineClearTime;
    private const float comboTimeLimit = 2.0f;
    
    private int nextPieceIndex = -1;
    [SerializeField] private GameObject[] NextMinoes;
    private bool first = true;
    private List<int> currentSet = new List<int>();
    public GameObject[] comboFx;
    public Transform fxSpawnPoint;
    
    public RectInt Bounds 
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) 
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
        Time.timeScale = 1f;
        GameOverUI.SetActive(false);
        ScoreManager.Instance.ComboUI.gameObject.SetActive(false);
    }
    
    private void InitializeSet()
    {
        currentSet.Clear();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            currentSet.Add(i);
        }
    }

    public void SpawnPiece()
    {
        if (nextPieceIndex == -1)
        {
            SetNextPiece();
        }

        TetrominoData data = tetrominoes[nextPieceIndex];

        activePiece.Initialize(this, spawnPosition, data);
        
        // Change Random System
        // int random = Random.Range(0, tetrominoes.Length);
        // TetrominoData data = tetrominoes[random];
        //
        // activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition)) 
        {
            Set(activePiece);
        } 
        else 
        {
            GameOver();
        }
        
        SetNextPiece();
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        // Do anything else you want on game over here..
        Time.timeScale = 0;
        GameOverUI.SetActive(true);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) 
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) 
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int linesCleared = 0; // 지워진 라인의 수를 추적하는 변수

        // 밑에서부터 위로 클리어
        while (row < bounds.yMax)
        {
            // 현재 라인이 클리어되지 않으면 다음 라인으로 넘어가지 않음
            // 위의 타일들이 라인 클리어 시 아래로 떨어지기 때문
            if (IsLineFull(row)) 
            {
                LineClear(row);
                linesCleared++; // 라인이 클리어 될 때마다 카운트 증가
            } 
            else 
                row++;
        }

        // 여러 줄이 한 번에 지워진 경우 로그 출력
        if (linesCleared > 1)
        {
            Debug.Log($"{linesCleared} 줄이 한 번에 지워졌습니다!");
            ScoreManager.Instance.Score += 200;
        }
        
        if (linesCleared > 0)
        {
            ScoreManager.Instance.ComboUI.gameObject.SetActive(true);
            ScoreManager.Instance.ComboUI.GetComponent<FadeOutText>().ResetOpacity();
            
            // 현재 시간과 마지막으로 라인을 지운 시간의 차이가 콤보 유지 시간 이내라면 콤보 카운트 증가
            if (Time.time - lastLineClearTime <= comboTimeLimit)
            {
                // 콤보 UI의 투명도를 초기화합니다.
                ScoreManager.Instance.ComboUI.GetComponent<FadeOutText>().ResetOpacity();
                ScoreManager.Instance.comboCount++;
                ScoreManager.Instance.ComboUI.GetComponent<FadeOutText>().StartFadingOut();
            }
            else // 콤보 유지 시간을 넘겼다면 콤보 카운트 리셋 후 1 증가
            {
                // 콤보 UI의 투명도를 초기화합니다.
                ScoreManager.Instance.ComboUI.GetComponent<FadeOutText>().ResetOpacity();
                ScoreManager.Instance.comboCount = 1;
                if (linesCleared > 1)
                {
                    ScoreManager.Instance.comboCount = 0;
                    ScoreManager.Instance.comboCount += linesCleared;
                }
                ScoreManager.Instance.ComboUI.GetComponent<FadeOutText>().StartFadingOut();
            }

            ScoreManager.Instance.Score += 50 * ScoreManager.Instance.comboCount;

            // 마지막으로 라인을 지운 시간 업데이트
            lastLineClearTime = Time.time;

            switch (ScoreManager.Instance.comboCount)
            {
                case 0:
                    break;
                case 1:
                    Instantiate(comboFx[0], fxSpawnPoint);
                    break;
                case 2:
                    Instantiate(comboFx[1], fxSpawnPoint);
                    break;
                case 3: case 4:
                    Instantiate(comboFx[2], fxSpawnPoint);
                    break;
                case 5: case 6: case 7: case 8:
                    Instantiate(comboFx[3], fxSpawnPoint);
                    break;
                default:
                    Instantiate(comboFx[4], fxSpawnPoint);
                    break;
            }
        }
    }
    
    private void SetNextPiece()
    {
        if (!first) NextMinoes[nextPieceIndex].gameObject.SetActive(false);
        else first = false;

        if (currentSet.Count == 0)
        {
            InitializeSet();
        }

        int setIndex = Random.Range(0, currentSet.Count);
        nextPieceIndex = currentSet[setIndex];
        currentSet.RemoveAt(setIndex);

        UpdateNextPieceUI();
        
    }
    
    private void UpdateNextPieceUI()
    {
        NextMinoes[nextPieceIndex].gameObject.SetActive(true);
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}
