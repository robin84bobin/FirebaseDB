using Assets.Scripts.UI.Windows.InfoWindows;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Windows
{
    public class GameOverWindow : BaseWindow 
    {
        public Text stateHeaderText;
        public Text scoreValueText;
        public InputField nameInputField;

        public static void Show()
        {
            App.UI.Show("GameOverWindow");
        }

        public void OnSaveScore()
        {
            if (string.IsNullOrEmpty(nameInputField.text)){
                InfoWindow.Show("Please enter your name");
                return;
            }

            //TODO
            //DataLoader.Instance.SaveResults(nameInputField.text,
        }

    }
}
