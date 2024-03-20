using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum ESquareStat
{

    Cross,
    Circle,
    Empty
}
public class NineChessBoard : MonoBehaviour
{
    // Start is called before the first frame update
    // enum EBoardState
    // {
    //     NotEnd,
    //     CircleWin,
    //     CrossWin,
    //     Draw
    // }


    // EBoardState boardState;
    Grid grid;
    ESquareStat[,] squareState = new ESquareStat[3, 3];

    public ESquareStat[,] SquareState{
        get{
            return squareState;
        }
    }

    public Vector2Int lastSquard; 
    
    Vector2Int lastGrid;
    Vector3 offset;

    public GameObject Circle;
    public GameObject Cross;

    public GameObject WinLine;

    public bool bBoardActive;

    public int count;


    void Start()
    {
        count = 0;

        grid = transform.GetComponent<Grid>();
        offset = grid.cellSize / 2;

        // boardState = EBoardState.NotEnd;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                squareState[i, j] = ESquareStat.Empty;
            }

        Invoke("Active",1f);

    }

    void Active(){
        bBoardActive=true;
    }


    // Update is called once per frame
    public void OnLeftMouseDown(Vector2 pos)
    {
        lastGrid = (Vector2Int)grid.WorldToCell(pos);
    }

    public bool OnLeftMouseUp(Vector2 pos)
    {
        if (lastGrid == (Vector2Int)grid.WorldToCell(pos))
        {
            if (lastGrid.x >= 0 && lastGrid.x <= 2 && lastGrid.y >= 0 && lastGrid.y <= 2){
                UpdateBoard(lastGrid.x, lastGrid.y);
                return true;
            }    
        }
        return false;
    }
    

    public bool IsWin()
    {
        ESquareStat state = GameManager.Instance.CurrentPlayer == 0 ? ESquareStat.Circle : ESquareStat.Cross;
        int x = lastSquard.x;
        int y = lastSquard.y;
        GameObject win = null;

        if (state == squareState[x, 0] && state == squareState[x, 1] && state == squareState[x, 2])
        {
            win = Instantiate(WinLine);
            win.transform.position = grid.CellToWorld(new Vector3Int(x, 1, 0)) + offset;
            win.transform.Rotate(0, 0, 90f);
        }

        if (state == squareState[0, y] && state == squareState[1, y] && state == squareState[2, y])
        {
            win = Instantiate(WinLine);
            win.transform.position = grid.CellToWorld(new Vector3Int(1, y , 0)) + offset;
        }

        if (x == y )
        {
            if (state == squareState[0, 0] && state == squareState[1, 1] && state == squareState[2, 2])
            {
                win = Instantiate(WinLine);
                win.transform.position = grid.CellToWorld(new Vector3Int(1, 1, 0)) + offset;
                win.transform.Rotate(0, 0, -135f);
            }
        }

        if(x - 1 == 1 - y)
        {

            if (state == squareState[0, 2] && state == squareState[1, 1] && state == squareState[2, 0])
            {
                win = Instantiate(WinLine);
                win.transform.position = grid.CellToWorld(new Vector3Int(1, 1, 0)) + offset;
                win.transform.Rotate(0, 0, -45f);
            }
        }

        if (win == null)
        {
            return false;
        }
        else
        {
            win.transform.parent = transform;
            win.SetActive(true);
            return true;
        }

    }

    public void UpdateBoard(int x, int y)
    {
        if(!bBoardActive){
            return;
        }

        if (squareState[x, y] == ESquareStat.Empty)
        {
            if (GameManager.Instance.CurrentPlayer == 0)
            {
                GameObject chess = Instantiate(Circle);
                chess.transform.position = grid.CellToWorld(new Vector3Int(x, y, 0)) + offset;
                chess.SetActive(true);
                chess.transform.parent = transform;
                squareState[x, y] = ESquareStat.Circle;
                SoundManager.Instance.PlaySound(1);
            }
            else
            {
                GameObject chess = Instantiate(Cross);
                chess.transform.position = grid.CellToWorld(new Vector3Int(x, y, 0)) + offset;
                chess.transform.parent = transform;
                chess.SetActive(true);
                squareState[x, y] = ESquareStat.Cross;
                SoundManager.Instance.PlaySound(0);
            }
            lastSquard=new Vector2Int(x,y);
            count++;
        }
    }
}
