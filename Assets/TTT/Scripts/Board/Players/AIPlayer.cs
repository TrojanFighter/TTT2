using System.Collections;
using UnityEngine;

namespace TTT.Board {
    // 下棋AI

    public class AIPlayer : MonoBehaviour {
        public static int playerId = 1; // 0 - 先手, 1 - 后手

        private bool[, ] ourBoard; // 我方棋子位置
        private bool[, ] foeBoard; // 对手的棋子位置

        private int continueAttack;
        private Transform boardTransform;

        private void Start () => PrepareNewGame ();

        private void PrepareNewGame () {
            boardTransform = BoardCreator.Instance.transform;

            ourBoard = playerId == 0 ? BoardCreator.circleBoard : BoardCreator.crossBoard;
            foeBoard = playerId != 0 ? BoardCreator.circleBoard : BoardCreator.crossBoard;

            StartCoroutine (TurnMonitor ());
        }

        private IEnumerator TurnMonitor () {
            while (true) {
                yield return null;

                if (GameController.round == playerId) {
                    yield return StartCoroutine (TakeCoroutineMove ());
                    GameController.round = (++GameController.round) % 2;
                }
            }
        }

        private IEnumerator TakeCoroutineMove () {
            yield return new WaitForSeconds (0.5f);

            if (!GameController.gameOver) {
                GameController.moves++;

                if (CheckIfAction (0)) // 检查AI是否获胜
                {
                    GameController.winnertext="AI";
                    GameController.roundEnded=true;
                    GameController.gameOver=true;
                    //GameController.DeclareWinner();
                    SoundManager.GetSoundEffect (0, 1f, 0.5f);
                    yield break;
                }
                // 分级应对策略
                if (CheckIfAction (1)) yield break; 
                if (CheckIfDefence ()) yield break; 
                if (CheckIfAttack ()) yield break; // 求胜
                if (CheckIfForced ()) yield break; 

                TakeFreeMove (HumanPlayer.row, HumanPlayer.col);
                GameController.winnertext="";
                GameController.currentplayerId=-1;
                GameController.roundEnded=true;
            }
        }

        private void TakeFreeMove (int row, int col) // 不需要应对的情况下一步
        {
            if (ourBoard[1, 1] == false) //  如果没有占据中间位置
            { 
                if (foeBoard[1, 1] == false) 
                    PlacePawn (1, 1); 
                else 
                {
                    if (ourBoard[0, 0] == false) 
                        PlacePawn (0, 0);
                    else
                    { // 阻止对方三连
                        if (GameController.moves == 4)
                            PlacePawn (0, 2);
                        else if (GameController.moves == 8) {
                            if (ourBoard[1, 0] == false && foeBoard[1, 0] == false)
                                PlacePawn (1, 0);
                            else {
                                if (!PlaceInCorner ()) 
                                    PlaceInSide (); 
                            }
                        }
                    }
                }
            } else if (ourBoard[1, 1] == true) // 如果占据了中间位置
            {
                if (row != 1) row = (row + 2) % 4;
                if (col != 1) col = (col + 2) % 4;

                if (ourBoard[row, col] == true)
                    PlaceInCorner ();
                else {
                    if (foeBoard[row, col] == true) PlaceInSide ();
                    else PlacePawn (row, col);
                }
            }
        }

        private bool PlaceInSide () 
        {
            if (ourBoard[0, 1] == false && foeBoard[0, 1] == false) return PlacePawn (0, 1);
            else if (ourBoard[1, 0] == false && foeBoard[1, 0] == false) return PlacePawn (1, 0);
            else if (ourBoard[1, 2] == false && foeBoard[1, 2] == false) return PlacePawn (1, 2);
            else if (ourBoard[2, 1] == false && foeBoard[2, 1] == false) return PlacePawn (2, 1);

            return false;
        }

        private bool PlaceInCorner () 
        {
            if (ourBoard[2, 2] == false && foeBoard[2, 2] == false) return PlacePawn (2, 2);
            else if (ourBoard[0, 2] == false && foeBoard[0, 2] == false) return PlacePawn (0, 2);
            else if (ourBoard[2, 0] == false && foeBoard[2, 0] == false) return PlacePawn (2, 0);
            else if (ourBoard[0, 0] == false && foeBoard[0, 0] == false) return PlacePawn (0, 0);

            return false;
        }

        private bool CheckIfForced () {
            if (CheckA (0, 1, 2, 0, 0, 0, 1, 0)) return true;
            else if (CheckA (0, 1, 2, 2, 0, 2, 1, 2)) return true;
            else if (CheckA (1, 2, 2, 0, 2, 2, 2, 1)) return true;
            else if (CheckA (1, 2, 0, 0, 0, 2, 0, 1)) return true;
            else if (CheckA (2, 1, 0, 0, 2, 0, 1, 0)) return true;
            else if (CheckA (2, 1, 0, 2, 2, 2, 1, 2)) return true;
            else if (CheckA (1, 0, 0, 2, 0, 0, 0, 1)) return true;
            else if (CheckA (1, 0, 2, 2, 2, 0, 2, 1)) return true;

            return false;
        }

        private bool CheckA (int a, int b, int c, int d, int e, int f, int g, int h) {
            if (foeBoard[a, b] == true && foeBoard[c, d] == true)
                if (ourBoard[e, f] == false && ourBoard[g, h] == false)
                    return PlacePawn (e, f); //Debug.Log("CheckA");

            return false;
        }

        private bool CheckIfDefence () {
            if (CheckB (0, 1, 1, 0, 0, 0)) return true;
            else if (CheckB (1, 0, 2, 1, 2, 0)) return true;
            else if (CheckB (0, 1, 1, 2, 0, 2)) return true;
            else if (CheckB (1, 2, 2, 1, 2, 2)) return true;

            return false;
        }

        private bool CheckB (int a, int b, int c, int d, int e, int f) {
            if (foeBoard[a, b] == true && foeBoard[c, d] == true)
                if (ourBoard[e, f] == false)
                    return PlacePawn (e, f); //Debug.Log("CheckB");

            return false;
        }

        private bool CheckIfAction (int mode) {
            int m = 0, n = 0, r = 0, c = 0;

            for (int i = 0; i < 3; i++)
                if (PerformLoop (mode, 3, i, ref m, ref n, ref r, ref c))
                    return true;

            for (int i = 0; i < 3; i++) 
                if (PerformLoop (mode, 2, i, ref m, ref n, ref r, ref c))
                    return true;
            
            if (PerformLoop (mode, 1, -1, ref m, ref n, ref r, ref c)) 
                return true;
            if (PerformLoop (mode, 0, -1, ref m, ref n, ref r, ref c)) 
                return true;

            return false;
        }

        private bool PerformLoop (int mode, int loop, int no, ref int m, ref int n, ref int r, ref int c) {
            m = n = 0;

            for (int i = 0; i < 3; i++)
                if (loop == 0) Detect (2 - i, i, ref m, ref n, ref r, ref c);
                else if (loop == 1) Detect (i, i, ref m, ref n, ref r, ref c);
            else if (loop == 2) Detect (i, no, ref m, ref n, ref r, ref c);
            else if (loop == 3) Detect (no, i, ref m, ref n, ref r, ref c);

            if (Check (mode, m, n, r, c))
                return true;

            return false;
        }

        private void Detect (int a, int b, ref int m, ref int n, ref int r, ref int c) {
            if (ourBoard[a, b] == true) m++;
            if (foeBoard[a, b] == true) n++;

            if (ourBoard[a, b] == false && foeBoard[a, b] == false) {
                r = a;
                c = b;
            }
        }

        private bool Check (int mode, int m, int n, int r, int c) {
            if (mode == 0) {
                if (m == 2 && n == 0) 
                    return PlacePawn (r, c); //Debug.Log("Check: Win");
            } else if (mode == 1)
                if (m == 0 && n == 2) 
                    return PlacePawn (r, c); //Debug.Log("Check: Defense");

            return false;
        }

        private bool CheckIfAttack () // 安全的时候进攻
        {
            if (GameController.moves == 3) {
                if (CheckC (1, 0, 0, 0, 1)) return true;
                else if (CheckC (1, 2, 2, 2, 2)) return true;
            }

            if (GameController.moves == 5) {
                if (continueAttack == 1) {
                    if (foeBoard[0, 1] == true) return PlacePawn (1, 0);
                    else return PlacePawn (0, 1);
                } else if (continueAttack == 2) {
                    if (foeBoard[1, 2] == true) return PlacePawn (2, 1);
                    else return PlacePawn (1, 2);
                }
            }

            return false;
        }

        private bool CheckC (int a, int b, int c, int d, int e) {
            if (ourBoard[1, 1] == true && (foeBoard[a, b] == true || foeBoard[b, a] == true)) {
                continueAttack = e;
                return PlacePawn (c, d);
            }

            return false;
        }

        private bool PlacePawn (int r, int c) {
            SoundManager.GetSoundEffect (1, 0.5f);

            ourBoard[r, c] = true;
            boardTransform.GetChild (3 * r + c).GetChild (playerId).gameObject.SetActive (true);
            boardTransform.GetChild (3 * r + c).GetComponent<BoxCollider> ().enabled = false;
            return true;
        }
    }
}