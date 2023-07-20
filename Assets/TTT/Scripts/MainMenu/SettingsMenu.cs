using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TTT.Board;

namespace TTT
{
    // Setting essential game parameters:
    // - self play
    // - who's playing first

    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private Toggle aiPlayToggle;
        [SerializeField] private Dropdown aiPlayDropdown;
        [SerializeField] private Toggle playingFirstToggle;
        [SerializeField] private Button startPlayBtn;

        private void Awake()
        {
            startPlayBtn.onClick.AddListener(LoadGame);
            aiPlayDropdown.onValueChanged.AddListener(AiPlayChanged);
        }

        private void AiPlayChanged(int isAI)
        {
            if(isAI==0){
                playingFirstToggle.gameObject.SetActive(true);
            }
            else{
                playingFirstToggle.gameObject.SetActive(false);
            }
        }

        private void LoadGame()
        {
            if (aiPlayDropdown.value>0)//!aiPlayToggle.isOn)    // we play with ourselves
            {
                HumanPlayer.selfPlay = true;
                HumanPlayer.playerId = 0;
                AIPlayer.playerId = -1;
            }
            else
            {
                HumanPlayer.selfPlay = false;

                if (playingFirstToggle.isOn)
                {
                    HumanPlayer.playerId = 0;
                    AIPlayer.playerId = 1;
                }
                else
                {
                    HumanPlayer.playerId = 1;
                    AIPlayer.playerId = 0;
                }
            }

            SceneManager.LoadScene("TTT");
        }
    }
}