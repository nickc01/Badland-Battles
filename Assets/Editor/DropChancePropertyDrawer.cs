using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(DropChance))]
public class DropChancePropertyDrawer : PropertyDrawer
{
	//Called whenver the drop chance property is displayed in the inspector
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//Begin the property
		EditorGUI.BeginProperty(position, label, property);

		//Add a prefix text to the property
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		//store the old indent value
		var oldIndent = EditorGUI.indentLevel;

		//Reset the indent
		EditorGUI.indentLevel = 0;

		var dropObjectRect = new Rect(position.x, position.y, position.width - 40f, position.height);
		var chanceRect = new Rect(position.x + position.width - 40f, position.y, 40f, position.height);
		
		//Draw the "Drop" object field
		EditorGUI.PropertyField(dropObjectRect, property.FindPropertyRelative("Drop"), GUIContent.none);
		//Draw the "Chance" float field
		EditorGUI.PropertyField(chanceRect, property.FindPropertyRelative("Chance"), GUIContent.none);

		//Set the indent back to what it was
		EditorGUI.indentLevel = oldIndent;

		//End the property
		EditorGUI.EndProperty();
	}
}