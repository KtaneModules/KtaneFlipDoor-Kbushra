using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;

public class SimpleModuleScript : MonoBehaviour {

	public KMAudio audio;
	public KMBombInfo info;
	public KMBombModule module;
	public KMSelectable[] Doors;
	public KMSelectable[] Buttons;
	static int ModuleIdCounter = 1;
	int ModuleId;

	public AudioSource correct;

	public int ans = 0;
	public int StageCur = 1;
	public int StageLim = 4;

	bool _isSolved = false;
	bool incorrect = false;

	void Awake() {
		ModuleId = ModuleIdCounter++;

		foreach (KMSelectable button in Doors)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { door(pressedButton); return false; };
		}
		foreach (KMSelectable button in Buttons)
		{
			KMSelectable pressedButton = button;
			button.OnInteract += delegate () { buttons(pressedButton); return false; };
		}
	}

	void Start ()
	{
		StageCur = 1;
		StageLim = 4;

		if (info.GetTime() % 600 > 300)
		{
			ans = 2;
			Log ("ans headstart is 2");
		}  
		if (info.GetTime() % 600 < 301) 
		{
			ans = 5;
			Log ("ans headstart is 5");
		}
		Invoke ("AnsAdder", 0);
	}

	void AnsAdder()
	{
		if (info.GetStrikes() > 0) 
		{
				ans = ans % 5;
			Log ("ans mod 5");
		}
			if (info.GetStrikes() > 2) 
		{
				ans = ans + 3;
			Log ("ans plus 3");
		}
			if (info.GetBatteryCount () > 4) 
		{
				ans = ans * 3;
			Log ("ans times 3");
		}
			if (info.GetBatteryHolderCount () > 2) 
		{
				ans = ans - 2;
			Log ("ans take away 2");
		}
		if (info.GetPortCount () > 1) 
		{
			ans = ans * 4;
			Log ("ans times 4");
		}
		if (info.GetPortPlateCount () > 2) 
		{
			ans = ans * 2;
			Log ("ans times 2");
		}
	}


	void door(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < Doors.Length; i++)
		{
			if (pressedButton == Doors[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (StageCur > 0) 
		{
			if (StageCur < 2) 
			{
				switch (buttonPosition)
				{
				    case 0:
					if(ans > 15)
					{
						incorrect = true;
						Log ("Strike! Ans is more than 15.");
					}
					break;
				    case 1:
					if(ans < 16)
					{
						incorrect = true;
						Log ("Strike! Ans is less than 16");
					}
					break;
				}

				if(incorrect)
				{
					incorrect = false;
					module.HandleStrike ();
					StageCur = 1;
				}
				else
				{
					correct.Play ();
					StageCur = StageCur + 1;
					Log ("stage increase");
				}
			}
		}
		if (StageCur > 2) 
		{
			if (StageCur < 4)
			{
				switch (buttonPosition)
				{
				case 0:
					if(ans > 30)
					{
						incorrect = true;
						Log ("Strike! Ans is more than 30.");
					}
					break;
				case 1:
					if(ans < 31)
					{
						incorrect = true;
						Log ("Strike! Ans is less than 31.");
					}
					break;
				}
				if(incorrect)
				{
					incorrect = false;
					module.HandleStrike ();
					StageCur = 1;
				}
				else
				{
					correct.Play ();
					StageCur = StageCur + 1;
					Log ("stage increase");
				}
			}
		}
	}

	void buttons(KMSelectable pressedButton)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform);
		int buttonPosition = new int();
		for(int i = 0; i < Buttons.Length; i++)
		{
			if (pressedButton == Buttons[i])
			{
				buttonPosition = i;
				break;
			}
		}

		if (StageCur < 3) 
		{
			if (StageCur > 1) 
			{
				switch (buttonPosition) 
				{
				    case 0:
					if (info.IsTwoFactorPresent () == true) 
					{
						incorrect = true;
						Log ("Strike! There is a two factor code.");
					}
					break;
				case 1:
					if (info.IsTwoFactorPresent () == false) 
					{
						incorrect = true;
						Log ("Strike! There is not a two factor code");
					}
					break;
				}
				if (incorrect) 
				{
					incorrect = false;
					module.HandleStrike ();
					StageCur = 1;
				}
				else
				{
					correct.Play ();
					StageCur = StageCur + 1;
					Log ("stage increase");
				}

			}
		}
		if (StageCur == StageLim) 
		{
			correct.Play ();
			module.HandlePass ();
			Log ("MODULE DEFUSED!");
		}
	}

	void Log(string message)
	{
		Debug.LogFormat("[The Door #{0}] {1}", ModuleId, message);
	}
}

