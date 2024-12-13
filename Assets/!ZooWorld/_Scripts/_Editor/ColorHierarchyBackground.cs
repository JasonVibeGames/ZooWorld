using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
public class ColorHierarchyBackground
{
    static ColorHierarchyBackground()
    {
        EditorApplication.hierarchyWindowItemOnGUI += ColorizeHierarchyItem;
    }

    private static void ColorizeHierarchyItem(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.name.Contains("---"))
        {
            Rect rect = new Rect(selectionRect);

            // Draw black border
            EditorGUI.DrawRect(rect, Color.black);

            // Draw gray background inside the border
            rect.x += 1;
            rect.width -= 1;
            rect.y += 1;
            rect.height -= 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1f)); // Gray color

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter; // Center the text
            Rect labelRect = new Rect(rect);
            labelRect.x += 2; // Adjust for padding
            labelRect.width -= 4; // Adjust for padding
            EditorGUI.LabelField(labelRect, gameObject.name.Replace("-", "").Trim(), style);
        }
    }
}
#endif