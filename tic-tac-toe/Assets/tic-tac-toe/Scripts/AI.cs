using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoSingleton<AI>
{
    // Start is called before the first frame update
    ESquareStat[,] squareState;

    ESquareStat[,,] squareStates;

    int[] difficultBase = { 2, 4, 6 };
    int[] difficultAdvance= { 2, 3, 4 };
    int level;

    int currCharacter;
    int AIChess = 1; //0是圈，1是叉
    int PlayerChess = 0;

    int count;

    int bestX, bestY, bestID;
    public void Active()
    {
        level = GameManager.Instance.AILevel;
        squareState = new ESquareStat[3, 3];
        squareStates = new ESquareStat[9, 3, 3];
    }
    public Vector2Int Play(NineChessBoard Board)
    {
        count = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                squareState[i, j] = Board.SquareState[i, j];
                if (squareState[i, j] != ESquareStat.Empty)
                {
                    count++;
                }
            }
        currCharacter = AIChess;
        MinimaxSearch(1);
        return new Vector2Int(bestX, bestY);

    }

    public Vector3Int Play(NineChessBoard mianBoard, NineChessBoard[] Boards, int boardID)
    {
        count = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                squareState[i, j] = mianBoard.SquareState[i, j];
            }

        for (int k = 0; k < 9; k++)
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    squareStates[k, i, j] = Boards[k].SquareState[i, j];
                    if (squareState[i, j] != ESquareStat.Empty)
                    {
                        count++;
                    }
                }
        currCharacter = AIChess;
        MinimaxSearch(1, boardID);
        return new Vector3Int(bestX, bestY, bestID);

    }

    int MinimaxSearch(int depth, int boardID)
    {
        int bestValue = currCharacter == AIChess ? int.MinValue : int.MaxValue;
        int value;

        if (count + depth > 81 || depth > difficultAdvance[level])
        {
            return 0;
        }

        bool bfull=true;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (squareStates[boardID, i, j] == ESquareStat.Empty)
                {
                    bfull = false;
                    break;
                }
            }



        for (int k = 0; k < 9; k++)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    int ID = bfull ? k : boardID;
                    if (squareStates[ID, i, j] != ESquareStat.Empty)
                    {
                        continue;
                    }

                    bfull = true;

                    if (currCharacter == AIChess)
                    {
                        squareStates[ID, i, j] = ESquareStat.Cross;
                        if (IsWin(i, j, ID) && squareState[ID / 3, ID % 3] == ESquareStat.Empty)
                        {
                            squareState[ID / 3, ID % 3] = ESquareStat.Cross;
                            if (IsWin(ID / 3, ID % 3))
                            {
                                value = 81 - depth;
                            }
                            else
                            {
                                currCharacter = PlayerChess;
                                value = MinimaxSearch(depth + 1, i * 3 + j);
                                currCharacter = AIChess;
                            }
                            squareState[ID / 3, ID % 3] = ESquareStat.Empty;
                        }
                        else
                        {
                            currCharacter = PlayerChess;
                            value = MinimaxSearch(depth + 1, i * 3 + j);
                            currCharacter = AIChess;
                        }

                        squareStates[ID, i, j] = ESquareStat.Empty;


                        if (value > bestValue)
                        {
                            bestValue = value;
                            if (depth == 1)
                            {
                                bestX = i;
                                bestY = j;
                                bestID = ID;
                            }
                        }
                        if (bestValue == 81 - depth)
                        {
                            break;
                        }

                    }
                    else
                    {

                        squareStates[ID,i, j] = ESquareStat.Circle;

                        if (IsWin(i, j, ID) && squareState[ID / 3, ID % 3] == ESquareStat.Empty)
                        {
                            squareState[ID / 3, ID % 3] = ESquareStat.Circle;
                            if (IsWin(ID / 3, ID % 3))
                            {
                                bestValue = Math.Min(bestValue, depth - 9);
                            }
                            else
                            {
                                currCharacter = AIChess;
                                bestValue = Math.Min(bestValue, MinimaxSearch(depth + 1, i * 3 + j));
                                currCharacter = PlayerChess;
                            }
                            squareState[ID / 3, ID % 3] = ESquareStat.Empty;
                        }
                        else
                        {
                            currCharacter = AIChess;
                            bestValue = Math.Min(bestValue, MinimaxSearch(depth + 1, i * 3 + j));
                            currCharacter = PlayerChess;
                        }
                        squareStates[ID, i, j] = ESquareStat.Empty;


                        if (bestValue == depth - 81)
                        {
                            break;
                        }

                    }
                }
                if(!bfull){
                    break;
                }
        }

        return bestValue;
    }

    int MinimaxSearch(int depth)
    {
        int bestValue = currCharacter == AIChess ? int.MinValue : int.MaxValue;
        int value;

        if (count + depth > 9 || depth > difficultBase[level])
        {
            return 0;
        }

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (squareState[i, j] != ESquareStat.Empty)
                {
                    continue;
                }

                if (currCharacter == AIChess)
                {
                    squareState[i, j] = ESquareStat.Cross;
                    if (IsWin(i, j))
                    {
                        value = 10 - depth;
                    }
                    else
                    {
                        currCharacter = PlayerChess;
                        value = MinimaxSearch(depth + 1);
                        currCharacter = AIChess;
                    }
                    squareState[i, j] = ESquareStat.Empty;


                    if (value > bestValue)
                    {
                        bestValue = value;
                        if (depth == 1)
                        {
                            bestX = i;
                            bestY = j;
                        }
                    }
                    if (bestValue == 9 - depth)
                    {
                        break;
                    }

                }
                else
                {

                    squareState[i, j] = ESquareStat.Circle;

                    if (IsWin(i, j))
                    {
                        bestValue = Math.Min(bestValue, depth - 9);
                    }
                    else
                    {
                        currCharacter = AIChess;
                        bestValue = Math.Min(bestValue, MinimaxSearch(depth + 1));
                        currCharacter = PlayerChess;
                    }
                    squareState[i, j] = ESquareStat.Empty;


                    if (bestValue == depth - 9)
                    {
                        break;
                    }

                }
            }
        return bestValue;
    }

    bool IsWin(int x, int y)
    {
        ESquareStat state = currCharacter == PlayerChess ? ESquareStat.Circle : ESquareStat.Cross;

        if (state == squareState[x, 0] && state == squareState[x, 1] && state == squareState[x, 2])
        {
            return true;
        }

        if (state == squareState[0, y] && state == squareState[1, y] && state == squareState[2, y])
        {
            return true;
        }

        if (x == y || x - 1 == 1 - y)
        {
            if (state == squareState[0, 0] && state == squareState[1, 1] && state == squareState[2, 2])
            {
                return true;
            }

            if (state == squareState[0, 2] && state == squareState[1, 1] && state == squareState[2, 0])
            {
                return true;
            }
        }

        return false;
    }

    bool IsWin(int x, int y, int boardID)
    {
        ESquareStat state = currCharacter == PlayerChess ? ESquareStat.Circle : ESquareStat.Cross;

        if (state == squareStates[boardID, x, 0] && state == squareStates[boardID, x, 1] && state == squareStates[boardID, x, 2])
        {
            return true;
        }

        if (state == squareStates[boardID, 0, y] && state == squareStates[boardID, 1, y] && state == squareStates[boardID, 2, y])
        {
            return true;
        }

        if (x == y || x - 1 == 1 - y)
        {
            if (state == squareStates[boardID, 0, 0] && state == squareStates[boardID, 1, 1] && state == squareStates[boardID, 2, 2])
            {
                return true;
            }

            if (state == squareStates[boardID, 0, 2] && state == squareStates[boardID, 1, 1] && state == squareStates[boardID, 2, 0])
            {
                return true;
            }
        }

        return false;
    }
}
