using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    // Start is called before the first frame update
    public enum EGameType
    {
        Base,
        Advance
    }


    enum EPlayerType
    {
        Computer,
        Battle

    }

    public EGameType gameType;
    EPlayerType playerType;
    public GameObject[] Plane;
    GameObject plane;
    public Vector2Int activeID;
    
    NineChessBoard activeBoard;
    NineChessBoard[] childrenBoards;
    bool bActiveBoardFull=false;

    NineChessBoard mainBoard;
    

    public int CurrentPlayer; //玩家1为0，持圈；电脑为1持x

    float delay; //用于动画延迟；
    public int PlayerChess = 0;
    public int AIChess = 1;
    public int AILevel = 0;


    public void SetLevel(int level)
    {
        AILevel = level;
    }

    public void SetType(int type){
        if(type==0){
            gameType=EGameType.Base;
        }
        else{
            gameType=EGameType.Advance;
        }
        
    }

    private void Start()
    {
        childrenBoards =new NineChessBoard[9];
    }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;

        }
        if (delay <= 0)
        {
            UIManager.Instance.SetPlayer(CurrentPlayer);
        }

        if (activeBoard?.bBoardActive == true && playerType == EPlayerType.Computer && CurrentPlayer == AIChess && delay <= 0)
        {
            AIPlay();
            if (!CheckWin())
            {
                SwitchPlayer();
            }

        }
    }

    public void Begin()
    {
        

        if(gameType==EGameType.Base){
            plane = Instantiate(Plane[0]);
            plane.SetActive(true);
            activeBoard = plane.transform.GetChild(0).gameObject.GetComponent<NineChessBoard>();
        }
        else{
            plane = Instantiate(Plane[1]);
            plane.SetActive(true);
            mainBoard=plane.transform.GetChild(0).gameObject.GetComponent<NineChessBoard>();

            for(int i=0;i<9;i++){
                childrenBoards[i]=plane.transform.GetChild(0).transform.GetChild(i).GetComponent<NineChessBoard>();
            }

            activeID=new Vector2Int(1,1);
            activeBoard = GetActiveBoard(activeID);
            activeBoard.transform.GetChild(0).gameObject.SetActive(true);
        }


        DecideFirst();

        AI.Instance.Active();

        MouseInput.Instance.OnLeftButtonDown += OnLeftMouseDown;
        MouseInput.Instance.OnLeftButtonUp += OnLeftMouseUp;
        SoundManager.Instance.PlaySound(2);

    }

    public void End()
    {
        MouseInput.Instance.OnLeftButtonDown -= OnLeftMouseDown;
        MouseInput.Instance.OnLeftButtonUp -= OnLeftMouseUp;
        Destroy(plane, 1f);
        UIManager.Instance.ResetUI(true);
    }

    public void Exit(){
        Application.Quit();
    }

    public void Reset()
    {
        
        UIManager.Instance.ResetUI(false);
        Destroy(plane);
        if (gameType == EGameType.Base)
        {
            plane = Instantiate(Plane[0]);
            plane.SetActive(true);
            activeBoard = plane.transform.GetChild(0).gameObject.GetComponent<NineChessBoard>();
        }
        else
        {
            plane = Instantiate(Plane[1]);
            plane.SetActive(true);
            mainBoard=plane.transform.GetChild(0).gameObject.GetComponent<NineChessBoard>();

            for(int i=0;i<9;i++){
                childrenBoards[i]=plane.transform.GetChild(0).transform.GetChild(i).GetComponent<NineChessBoard>();
            }

            activeID=new Vector2Int(1,1);
            activeBoard = GetActiveBoard(activeID);
            activeBoard.transform.GetChild(0).gameObject.SetActive(true);

        }


        DecideFirst();
        AI.Instance.Active();

        SoundManager.Instance.PlaySound(2);
    }

    NineChessBoard GetActiveBoard(Vector2Int ID){
        return childrenBoards[ID.x*3+ID.y];
    }
    private void DecideFirst()
    {
        CurrentPlayer = Random.Range(0, 2);
    }


    public void OnLeftMouseDown(Vector2 pos)
    {
        activeBoard.OnLeftMouseDown(pos);
        if (bActiveBoardFull && gameType == EGameType.Advance){
            for (int i = 0; i < 9; i++)
                {
                    childrenBoards[i].OnLeftMouseDown(pos);
                }
        }
    }

    public void OnLeftMouseUp(Vector2 pos)
    {
        if (activeBoard?.bBoardActive == true && CurrentPlayer == PlayerChess && delay <= 0)
        {
            if (bActiveBoardFull && gameType == EGameType.Advance)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (childrenBoards[i].OnLeftMouseUp(pos))
                    {
                        activeBoard = childrenBoards[i];
                        activeID = new Vector2Int(i / 3, i % 3);
                        if (CheckWin())
                        {
                            SwitchPlayer();
                        }
                        break;
                    }
                }
            }
            else
            {
                if (activeBoard.OnLeftMouseUp(pos) && !CheckWin())
                {
                    SwitchPlayer();
                }
            }
        }

    }


    void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == PlayerChess ? AIChess : PlayerChess;
        delay = 0.85f;

        if (gameType == EGameType.Advance)
        {
            
            activeID = activeBoard.lastSquard;
            activeBoard = GetActiveBoard(activeID);
            DisableChoose();
            if (activeBoard.count == 9)
            {
                bActiveBoardFull = true;
                for(int i=0;i<9;i++){
                    if(i!=activeID.x*3+activeID.y){
                        childrenBoards[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }   
            else
            {
                bActiveBoardFull = false;
                activeBoard.transform.GetChild(0).gameObject.SetActive(true);
            }
        }

    }

    void DisableChoose(){
        for(int i=0;i<9;i++){
            childrenBoards[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    void AIPlay()
    {
        if (gameType == EGameType.Base)
        {
            Vector2Int aiSquard = AI.Instance.Play(activeBoard);
            activeBoard.UpdateBoard(aiSquard.x, aiSquard.y);
        }
        else
        {
            Vector3Int aiSquard = AI.Instance.Play(mainBoard, childrenBoards, activeID.x * 3 + activeID.y);
            childrenBoards[aiSquard.z].UpdateBoard(aiSquard.x, aiSquard.y);
            activeBoard = childrenBoards[aiSquard.z];
            activeID = new Vector2Int(aiSquard.z/3, aiSquard.z%3);
        }

    }

    private bool CheckWin()
    {
        if (gameType == EGameType.Base)
        {
            if (activeBoard.IsWin())
            {
                UIManager.Instance.PlayResult(CurrentPlayer, false);
                activeBoard.bBoardActive = false;
                CurrentPlayer = -1;
                return true;
            }
            else if (activeBoard.count == 9)
            {
                UIManager.Instance.PlayResult(CurrentPlayer, true);
                activeBoard.bBoardActive = false;
                CurrentPlayer = -1;
                return true;
            }
            return false;
        }
        else
        {
            if (activeBoard.IsWin())
            {
                mainBoard.UpdateBoard(activeID.x,activeID.y);
                if(mainBoard.IsWin()){
                    UIManager.Instance.PlayResult(CurrentPlayer, false);
                    activeBoard.bBoardActive = false;
                    CurrentPlayer = -1;
                    return true;
                }
                else if(mainBoard.count==9)
                {
                    UIManager.Instance.PlayResult(CurrentPlayer, true);
                    activeBoard.bBoardActive = false;
                    CurrentPlayer = -1;
                    return true;
                }
            }
            return false;
        }

    }



}
