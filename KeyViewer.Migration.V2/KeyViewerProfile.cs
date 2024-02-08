using System.Collections.Generic;
using UnityEngine;

namespace KeyViewer.Migration.V2;

public class KeyViewerProfile
{
	public Color PressedOutlineColor;

	public Color ReleasedOutlineColor;

	public Color PressedBackgroundColor;

	public Color ReleasedBackgroundColor;

	public Color PressedTextColor;

	public Color ReleasedTextColor;

	public string Name { get; set; }

	public List<KeyCode> ActiveKeys { get; set; } = new List<KeyCode>();


	public bool ViewerOnlyGameplay { get; set; }

	public bool AnimateKeys { get; set; } = true;


	public bool ShowKeyPressTotal { get; set; } = true;


	public float KeyViewerSize { get; set; } = 100f;


	public float KeyViewerXPos { get; set; } = 0.89f;


	public float KeyViewerYPos { get; set; } = 0.03f;


	public KeyViewerProfile()
	{
		PressedOutlineColor = Color.white;
		ReleasedOutlineColor = Color.white;
		PressedBackgroundColor = Color.white;
		ReleasedBackgroundColor = Color.black.WithAlpha(0.4f);
		PressedTextColor = Color.black;
		ReleasedTextColor = Color.white;
	}
}
