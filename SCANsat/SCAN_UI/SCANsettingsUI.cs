﻿#region license
/* 
 *  [Scientific Committee on Advanced Navigation]
 * 			S.C.A.N. Satellite
 *
 * SCANsat - Settings menu window object
 * 
 * Copyright (c)2013 damny;
 * Copyright (c)2014 David Grandy <david.grandy@gmail.com>;
 * Copyright (c)2014 technogeeky <technogeeky@gmail.com>;
 * Copyright (c)2014 (Your Name Here) <your email here>; see LICENSE.txt for licensing details.
 *
 */
#endregion

using SCANsat.SCAN_Toolbar;
using SCANsat.SCAN_Data;
using SCANsat.SCAN_UI.UI_Framework;
using SCANsat.SCAN_Platform;
using SCANsat.SCAN_Platform.Palettes;
using UnityEngine;

using palette = SCANsat.SCAN_UI.UI_Framework.SCANpalette;

namespace SCANsat.SCAN_UI
{
	class SCANsettingsUI: SCAN_MBW
	{
		private string settingsHelpAnomalies = "";
		private string settingsHelpBackground = "";
		private string settingsHelpTimeWarp = "";
		private string settingsHelpGroundTracks = "";
		private string settingsHelpGroundTracksActive = "";
		private string settingsHelpOverlayTooltips = "";
		private string settingsHelpWindowTooltips = "";
		private string settingsHelpStockToolbar = "";
		private string settingsHelpMechJeb = "";
		private string settingsHelpResetWindows = "";
		private string settingsHelpResetPlanetData = "";
		private string settingsHelpResetAllData = "";
		private string settingsHelpVessels = "";
		private string settingsHelpGreyScale = "";
		private string settingsHelpExportCSV = "";
		private string settingsHelpSetMapWidth = "";

		/* UI: a list of glyphs that are used for something */
		private string[] exmarks = { "✗", "✘", "×", "✖", "x", "X", "∇", "☉", "★", "*", "•", "º", "+" };

		/* UI: time warp names and settings */
		private string[] twnames = { "Off", "Low", "Medium", "High" };
		private int[] twvals = { 1, 6, 9, 15 };
		private bool popup, warningBoxOne, warningBoxAll, controlLock;
		private Rect warningRect;
		private const string lockID = "settingLockID";
		private bool oldTooltips, stockToolbar;
		private string exportSize = "";

		internal readonly static Rect defaultRect = new Rect(Screen.width - (Screen.width / 2) - 180, 100, 360, 300);

		protected override void Awake()
		{
			WindowCaption = "S.C.A.N. Settings";
			WindowRect = defaultRect;
			WindowStyle = SCANskins.SCAN_window;
			WindowOptions = new GUILayoutOption[2] { GUILayout.Width(360), GUILayout.Height(300) };
			Visible = false;
			DragEnabled = true;
			TooltipMouseOffset = new Vector2d(-10, -25);
			TooltipMaxWidth = 350;
			TooltipDisplayForSecs = 60;
			ClampToScreenOffset = new RectOffset(-280, -280, -600, -600);

			SCAN_SkinsLibrary.SetCurrent("SCAN_Unity");

			removeControlLocks();
		}

		protected override void OnDestroy()
		{
			removeControlLocks();
			TooltipsEnabled = false;
		}

		internal void removeControlLocks()
		{
			InputLockManager.RemoveControlLock(lockID);
			controlLock = false;
		}

		protected override void Start()
		{
			oldTooltips = SCANcontroller.controller.toolTips;
			TooltipsEnabled = false;
			stockToolbar = SCANcontroller.controller.useStockAppLauncher;

			if (SCANconfigLoader.languagePack != null)
				loadStrings();
		}

		private void loadStrings()
		{
			settingsHelpAnomalies = SCANconfigLoader.languagePack.settingsHelpAnomalies;
			settingsHelpBackground = SCANconfigLoader.languagePack.settingsHelpBackground;
			settingsHelpTimeWarp = SCANconfigLoader.languagePack.settingsHelpTimeWarp;
			settingsHelpGroundTracks = SCANconfigLoader.languagePack.settingsHelpGroundTracks;
			settingsHelpGroundTracksActive = SCANconfigLoader.languagePack.settingsHelpGroundTracksActive;
			settingsHelpOverlayTooltips = SCANconfigLoader.languagePack.settingsHelpOverlayTooltips;
			settingsHelpWindowTooltips = SCANconfigLoader.languagePack.settingsHelpWindowTooltips;
			settingsHelpStockToolbar = SCANconfigLoader.languagePack.settingsHelpStockToolbar;
			settingsHelpMechJeb = SCANconfigLoader.languagePack.settingsHelpMechJeb;
			settingsHelpResetWindows = SCANconfigLoader.languagePack.settingsHelpResetWindows;
			settingsHelpResetPlanetData = SCANconfigLoader.languagePack.settingsHelpResetPlanetData;
			settingsHelpResetAllData = SCANconfigLoader.languagePack.settingsHelpResetAllData;
			settingsHelpVessels = SCANconfigLoader.languagePack.settingsHelpVesselsSensorsPasses;
			settingsHelpGreyScale = SCANconfigLoader.languagePack.settingsHelpGreyScale;
			settingsHelpExportCSV = SCANconfigLoader.languagePack.settingsHelpExportCSV;
			settingsHelpSetMapWidth = SCANconfigLoader.languagePack.settingsHelpSetMapWidth;
		}

		protected override void DrawWindowPre(int id)
		{
			//Lock space center click through
			if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.CAMERACONTROLS | ControlTypes.KSC_ALL, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
				{
					removeControlLocks();
				}
			}

			//Lock tracking scene click through
			if (HighLogic.LoadedScene == GameScenes.TRACKSTATION)
			{
				Vector2 mousePos = Input.mousePosition;
				mousePos.y = Screen.height - mousePos.y;
				if (WindowRect.Contains(mousePos) && !controlLock)
				{
					InputLockManager.SetControlLock(ControlTypes.TRACKINGSTATION_UI, lockID);
					controlLock = true;
				}
				else if (!WindowRect.Contains(mousePos) && controlLock)
				{
					InputLockManager.RemoveControlLock(lockID);
					controlLock = false;
				}
			}

			if (!popup)
			{
				warningBoxOne = false;
				warningBoxAll = false;
			}
		}

		protected override void DrawWindow(int id)
		{
			versionLabel(id);				/* Standard version label and close button */
			closeBox(id);

			growS();
				gui_settings_resources(id);				/* resource scanning options */
				gui_settings_xmarks(id); 				/* X marker selection */
				gui_settings_toggle_body_scanning(id);	/* background and body scanning toggles */
				gui_settings_timewarp(id);				/* time warp resolution settings */
				gui_settings_numbers(id);				/* sensor/scanning		statistics */
				gui_settings_window_resets_tooltips(id);/* reset windows and positions and toggle tooltips*/
				gui_settings_export_options(id);
				gui_settings_data_resets(id);			/* reset data and/or reset resources */
				# if DEBUG
					gui_settings_window_mapFill(id);	/* debug option to fill in maps */
				#endif
			stopS();

			warningBox(id);						/* Warning box for deleting map data */
		}

		protected override void DrawWindowPost(int id)
		{
			if (popup && Event.current.type == EventType.mouseDown && !warningRect.Contains(Event.current.mousePosition))
			{
				popup = false;
			}

			if (oldTooltips != SCANcontroller.controller.toolTips)
			{
				oldTooltips = SCANcontroller.controller.toolTips;
				TooltipsEnabled = SCANcontroller.controller.toolTips;
				if (HighLogic.LoadedSceneIsFlight)
				{
					SCANcontroller.controller.BigMap.TooltipsEnabled = SCANcontroller.controller.toolTips;
					SCANcontroller.controller.mainMap.TooltipsEnabled = SCANcontroller.controller.toolTips;
					if (SCANcontroller.controller.BigMap.spotMap != null)
						SCANcontroller.controller.BigMap.spotMap.TooltipsEnabled = SCANcontroller.controller.toolTips;
				}
				if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION)
				{
					SCANcontroller.controller.kscMap.TooltipsEnabled = SCANcontroller.controller.toolTips;
					if (SCANcontroller.controller.kscMap.spotMap != null)
						SCANcontroller.controller.kscMap.spotMap.TooltipsEnabled = SCANcontroller.controller.toolTips;
				}
			}

			if (stockToolbar != SCANcontroller.controller.useStockAppLauncher)
			{
				stockToolbar = SCANcontroller.controller.useStockAppLauncher;
				if (stockToolbar)
					SCANcontroller.controller.appLauncher = gameObject.AddComponent<SCANappLauncher>();
				else
					Destroy(SCANcontroller.controller.appLauncher);
			}
		}

		//Draw the version label in the upper left corner
		private void versionLabel(int id)
		{
			Rect r = new Rect(6, 0, 50, 18);
			GUI.Label(r, SCANmainMenuLoader.SCANsatVersion, SCANskins.SCAN_whiteReadoutLabel);
		}

		//Draw the close button in the upper right corner
		private void closeBox(int id)
		{
			Rect r = new Rect(WindowRect.width - 42, 1, 18, 18);
			if (GUI.Button(r, textWithTT("?", "Show Help Tips"), SCANskins.SCAN_closeButton))
			{
				TooltipsEnabled = !TooltipsEnabled;
			}

			r.x += 22;

			if (GUI.Button(r, SCANcontroller.controller.closeBox, SCANskins.SCAN_closeButton))
			{
				removeControlLocks();
				Visible = false;
				TooltipsEnabled = false;
			}
		}

		//Choose anomaly marker icon
		private void gui_settings_xmarks(int id)
		{
			fillS(8);
			GUILayout.Label(textWithTT("Anomaly Marker", settingsHelpAnomalies), SCANskins.SCAN_headline);
			growE();
			for (int i = 0; i < exmarks.Length; ++i)
			{
				if (SCANcontroller.controller.anomalyMarker == exmarks[i])
				{
					if (GUILayout.Button(textWithTT(exmarks[i], settingsHelpAnomalies), SCANskins.SCAN_closeButton))
						SCANcontroller.controller.anomalyMarker = exmarks[i];
				}
				else
				{
					if (GUILayout.Button(textWithTT(exmarks[i], settingsHelpAnomalies), SCANskins.SCAN_buttonBorderless))
						SCANcontroller.controller.anomalyMarker = exmarks[i];
				}
			}
			stopE();
			fillS(16);
		}

		//Control background scanning options
		private void gui_settings_toggle_body_scanning(int id)
		{

			GUILayout.Label(textWithTT("Background Scanning", settingsHelpBackground), SCANskins.SCAN_headline);
			// scan background
			SCANcontroller.controller.scan_background = GUILayout.Toggle(SCANcontroller.controller.scan_background, textWithTT("Scan all active celestials", settingsHelpBackground), SCANskins.SCAN_settingsToggle);
			// scanning for individual SoIs
			growE();
			int count = 0;
			foreach (SCANdata data in SCANcontroller.controller.GetAllData)
			{
				if (count == 0) growS();
				data.Disabled = !GUILayout.Toggle(!data.Disabled, textWithTT(string.Format("{0} ({1:N1}%)", data.Body.name, SCANUtil.getCoveragePercentage(data, SCANtype.Nothing)), settingsHelpBackground), SCANskins.SCAN_settingsToggle);
				switch (count)
				{
					case 5: stopS(); count = 0; break;
					default: ++count; break;
				}
			}
			if (count != 0)
				stopS(); ;
			stopE();
		}

		//Control scanning resolution
		private void gui_settings_timewarp(int id)
		{
			GUILayout.Label(textWithTT("Time Warp Resolution", settingsHelpTimeWarp), SCANskins.SCAN_headline);
			growE();

			for (int i = 0; i < twnames.Length; ++i)
			{
				if (SCANcontroller.controller.timeWarpResolution == twvals[i])
				{
					if (GUILayout.Button(textWithTT(twnames[i], settingsHelpTimeWarp), SCANskins.SCAN_buttonActive))
						SCANcontroller.controller.timeWarpResolution = twvals[i];
				}
				else
				{
					if (GUILayout.Button(textWithTT(twnames[i], settingsHelpTimeWarp)))
						SCANcontroller.controller.timeWarpResolution = twvals[i];
				}
			}
			stopE();
			fillS(8);
		}

		//Display the total number of SCANsat sensors and scanning passes
		/* Needs to be clarified for users */
		private void gui_settings_numbers(int id)
		{
			string s = 	"Vessels: " + SCANcontroller.controller.ActiveVessels.ToString() +
						" Sensors: " + SCANcontroller.controller.ActiveSensors +
						" Passes: " + SCANcontroller.controller.ActualPasses.ToString();
			GUILayout.Label(textWithTT(s, settingsHelpVessels), SCANskins.SCAN_whiteReadoutLabel);
			fillS(16);
		}

		//Export options
		private void gui_settings_export_options(int id)
		{
			GUILayout.Label("Export Options", SCANskins.SCAN_headline);

			growE();
				fillS();
				GUILayout.Label(textWithTT("Map Width: " + SCANcontroller.controller.map_width, settingsHelpSetMapWidth), SCANskins.SCAN_settingsGreyLabel, GUILayout.Width(110));

				exportSize = GUILayout.TextField(exportSize, 4, GUILayout.Width(75));

				if (GUILayout.Button(textWithTT("Set", settingsHelpSetMapWidth), GUILayout.Width(50)))
				{
					if (SCANcontroller.controller.BigMap == null)
						return;

					int i = 0;

					if (int.TryParse(exportSize, out i))
					{
						if (i <= SCANcontroller.controller.BigMap._WindowSize_Min.x)
							i = (int)SCANcontroller.controller.BigMap._WindowSize_Min.x;

						SCANcontroller.controller.BigMap.setMapWidth(i);
					}
				}
				fillS();
			stopE();

			growE();
				SCANcontroller.controller.trueGreyScale = GUILayout.Toggle(SCANcontroller.controller.trueGreyScale, textWithTT("Use True Grey Scale", settingsHelpGreyScale), SCANskins.SCAN_settingsToggle);

				SCANcontroller.controller.exportCSV = GUILayout.Toggle(SCANcontroller.controller.exportCSV, textWithTT("Export .csv Data File", settingsHelpExportCSV), SCANskins.SCAN_settingsToggle);
			stopE();
		}

		//Reset databases
		private void gui_settings_data_resets(int id)
		{
			CelestialBody thisBody = getTargetBody();

			if (thisBody == null)
				return;

			GUILayout.Label("Data Management", SCANskins.SCAN_headline);
			growE();
			if (popup)
			{
				GUILayout.Label("Reset map of " + thisBody.theName, SCANskins.SCAN_button);
				GUILayout.Label("Reset <b>all</b> data", SCANskins.SCAN_button);
			}
			else
			{
				if (GUILayout.Button(textWithTT("Reset map of " + thisBody.theName, settingsHelpResetPlanetData)))
				{
					popup = !popup;
					warningBoxOne = !warningBoxOne;
				}
				if (GUILayout.Button(textWithTT("Reset <b>all</b> data", settingsHelpResetAllData)))
				{
					popup = !popup;
					warningBoxAll = !warningBoxAll;
				}
			}
			stopE();
			fillS(8);
		}

		//Resets all window positions, tooltip toggle
		private void gui_settings_window_resets_tooltips(int id)
		{
			GUILayout.Label("Settings", SCANskins.SCAN_headline);
			growE();
				SCANcontroller.controller.groundTracks = GUILayout.Toggle(SCANcontroller.controller.groundTracks, textWithTT("Show Ground Tracks", settingsHelpGroundTracks), SCANskins.SCAN_settingsToggle);

				if (SCANcontroller.controller.groundTracks)
					SCANcontroller.controller.groundTrackActiveOnly = GUILayout.Toggle(SCANcontroller.controller.groundTrackActiveOnly, textWithTT("Active Vessel Only", settingsHelpGroundTracksActive), SCANskins.SCAN_settingsToggle);
			stopE();
			growE();
				SCANcontroller.controller.planetaryOverlayTooltips = GUILayout.Toggle(SCANcontroller.controller.planetaryOverlayTooltips, textWithTT("Planetary Overlay Tooltips", settingsHelpOverlayTooltips), SCANskins.SCAN_settingsToggle);

				SCANcontroller.controller.toolTips = GUILayout.Toggle(SCANcontroller.controller.toolTips, textWithTT("Window Tooltips", settingsHelpWindowTooltips), SCANskins.SCAN_settingsToggle);
			stopE();

			growE();
				SCANcontroller.controller.useStockAppLauncher = GUILayout.Toggle(SCANcontroller.controller.useStockAppLauncher, textWithTT("Stock Toolbar", settingsHelpStockToolbar), SCANskins.SCAN_settingsToggle);

				if (SCANmainMenuLoader.MechJebLoaded)
					SCANcontroller.controller.mechJebTargetSelection = GUILayout.Toggle(SCANcontroller.controller.mechJebTargetSelection, textWithTT("MechJeb Target Selection", settingsHelpMechJeb), SCANskins.SCAN_settingsToggle);
			stopE();
			fillS(8);
			if (popup)
			{
				GUILayout.Label("Reset window positions", SCANskins.SCAN_button);
			}
			else
			{
				if (GUILayout.Button(textWithTT("Reset window positions", settingsHelpResetWindows)))
				{
					if (HighLogic.LoadedSceneIsFlight)
					{
						SCANuiUtil.resetMainMapPos();
						SCANuiUtil.resetBigMapPos();
						SCANuiUtil.resetInstUIPos();
						SCANuiUtil.resetSettingsUIPos();
						SCANuiUtil.resetColorMapPos();
					}
					else
					{
						SCANuiUtil.resetKSCMapPos();
						SCANuiUtil.resetColorMapPos();
						SCANuiUtil.resetSettingsUIPos();
					}
				}
			}
			fillS(8);
		}

		private void gui_settings_resources(int id)
		{
			if (SCANcontroller.controller.resourceSettings == null)
				return;

			if (GUILayout.Button("Resource Settings Window"))
			{
				SCANcontroller.controller.resourceSettings.Visible = !SCANcontroller.controller.resourceSettings.Visible;
			}
		}

		//Debugging option to fill in SCAN maps
		private void gui_settings_window_mapFill(int id)
		{
			growE();
			CelestialBody thisBody = getTargetBody();

			if (thisBody == null)
				return;
			if (GUILayout.Button("Fill SCAN map of " + thisBody.theName))
			{
				SCANdata data = SCANUtil.getData(thisBody);
				if (data == null)
				{
					data = new SCANdata(thisBody);
					SCANcontroller.controller.addToBodyData(thisBody, data);
				}
				data.fillMap();
			}
			if (GUILayout.Button("Fill SCAN map for all planets"))
			{
				foreach (CelestialBody b in FlightGlobals.Bodies)
				{
					SCANdata data = SCANUtil.getData(b);
					if (data == null)
					{
						data = new SCANdata(b);
						SCANcontroller.controller.addToBodyData(b, data);
					}
					data.fillMap();
				}
			}
			stopE();
			fillS(8);
		}

		//Confirmation boxes for map resets
		private void warningBox(int id)
		{
			if (popup)
			{
				if (warningBoxOne)
				{
					CelestialBody thisBody = getTargetBody();

					if (thisBody == null)
					{
						popup = false;
						return;
					}

					warningRect = new Rect(WindowRect.width - (WindowRect.width / 2) - 150, WindowRect.height - 125, 300, 90);
					GUI.Box(warningRect, "");
					Rect r = new Rect(warningRect.x + 10, warningRect.y + 5, 280, 40);
					GUI.Label(r, "Erase all data for " + thisBody.theName + "?", SCANskins.SCAN_headlineSmall);
					r.x += 90;
					r.y += 45;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", SCANskins.SCAN_buttonWarning))
					{
						popup = false;
						warningBoxOne = false;
						SCANdata data = SCANUtil.getData(thisBody);
						if (data != null)
							data.reset();
					}
				}
				else if (warningBoxAll)
				{
					warningRect = new Rect(WindowRect.width - (WindowRect.width / 2) - 120, WindowRect.height - 160, 240, 90);
					GUI.Box(warningRect, "");
					Rect r = new Rect(warningRect.x + 10, warningRect.y + 5, 220, 40);
					GUI.Label(r, "Erase <b>all</b> data ?", SCANskins.SCAN_headlineSmall);
					r.x += 70;
					r.y += 45;
					r.width = 80;
					r.height = 30;
					if (GUI.Button(r, "Confirm", SCANskins.SCAN_buttonWarning))
					{
						popup = false;
						warningBoxAll = false;
						foreach (SCANdata data in SCANcontroller.controller.GetAllData)
						{
							data.reset();
						}
					}
				}
				else
					popup = false;
			}
		}

		private CelestialBody getTargetBody()
		{
			switch (HighLogic.LoadedScene)
			{
				case GameScenes.FLIGHT:
					return FlightGlobals.currentMainBody;
				case GameScenes.SPACECENTER:
					return Planetarium.fetch.Home;
				case GameScenes.TRACKSTATION:
					return SCANUtil.getTargetBody(MapView.MapCamera.target);
				default:
					return null;
			}
		}

	}
}
