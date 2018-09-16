using UnityEngine;
using UnityEngine.UI;

public static class UIExtensions
{

    public static void SelectValue(this Dropdown dropdown, string value)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == value)
            {
                dropdown.value = i;
                return;
            }
                
        }

        Debug.LogError("No item in dropdown found: " + value);
    }
}
