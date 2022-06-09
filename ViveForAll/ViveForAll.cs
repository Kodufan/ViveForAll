using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

public class ViveForAll : NeosMod
{
	public override string Author => "Kodu";
	public override string Link => "https://github.com/Kodufan/ViveForAll";
	public override string Name => "ViveForAll";
	public override string Version => "1.0.0";

	private static InputGroup
		dualControllerSmoothLocomotionInputGroup,
		dualControllerSmooth3AxisLocomotionInputGroup,
		dualControllerPhotoActionsInputGroup,
		dualControllerGlobalActionsInputGroup,
		dualControllerUndoInputGroup,
		desktopPhotoActionsInputGroup,
		desktopUndoInputGroup;
	private static TrackedObject leftController, rightController;

	private enum Setting
	{
		Jump,
		MuteToggle,
		Photo,
		Undo,
		None
	}

	private enum ControllerType
	{
		Index,
		Touch,
		Reverb,
		None
	}

	public static ModConfiguration config;

	[AutoRegisterConfigKey]
	private static ModConfigurationKey<Setting> LeftHand = new ModConfigurationKey<Setting>("Left Hand Action", "Left Hand Action", () => Setting.MuteToggle);

	[AutoRegisterConfigKey]
	private static ModConfigurationKey<Setting> RightHand = new ModConfigurationKey<Setting>("Right Hand Action", "Right Hand Action", () => Setting.Jump);

	public override void OnEngineInit()
	{
		config = GetConfiguration();
		config.Save(true);

		Harmony harmony = new Harmony($"dev.{Author}.{Name}");
		harmony.PatchAll();


	}

	[HarmonyPatch(typeof(IndexController), "Bind")]
	class IndexControllerBindPatch
	{
		[HarmonyPostfix]
		static void Postfix(InputGroup group, IndexController __instance)
		{
			Chirality? side = group.Side;
			Chirality side2 = __instance.Side;
			if (!(side.GetValueOrDefault() == side2 & side != null))
			{
				return;
			}

			// Captures controller objects for later use.
			if (leftController == null && side2 == Chirality.Left)
			{
				leftController = __instance;
			}
			else if (rightController == null && side2 == Chirality.Right)
			{
				rightController = __instance;
			}

			CommonToolInputs commonToolInputs = group as CommonToolInputs;
			if (commonToolInputs != null)
			{
				// Clears bindings from the vanilla method
				commonToolInputs.ClearBindings();
				commonToolInputs.Interact.AddBinding(__instance.TriggerClick);
				commonToolInputs.Secondary.AddBinding(__instance.TouchpadPress);
				commonToolInputs.Grab.AddBinding(__instance.GripClick);
				commonToolInputs.Menu.AddBinding(__instance.ButtonB);
				commonToolInputs.Strength.AddBinding(__instance.Trigger);
				commonToolInputs.Axis.AddBinding(__instance.Touchpad);

				// Removing this binding will activate one button mode for whatever Menu is bound to.
				// commonToolInputs.UserspaceToggle.AddBinding(__instance.ButtonA);
			}
		}
	}

	[HarmonyPatch(typeof(IndexController), "BindNodeActions")]
	class IndexControllersBindNodeActionsPatch
	{
		[HarmonyPrefix]
		public static void Postfix(IInputNode node, string name = null)
		{
			if (!(config.GetValue(LeftHand) == Setting.Jump) && !(config.GetValue(RightHand) == Setting.Jump)) return;
			bool both = (config.GetValue(LeftHand) == Setting.Jump) && (config.GetValue(RightHand) == Setting.Jump);
			Chirality side = (config.GetValue(LeftHand) == Setting.Jump) ? Chirality.Left : Chirality.Right;


			IDualAxisInputNode dualAxisInputNode = node as IDualAxisInputNode;
			if (dualAxisInputNode == null)
			{
				AnyInput anyInput = node as AnyInput;
				if (anyInput != null && name == "Jump")
				{
					if (side == Chirality.Left)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Left, "ButtonA", false));
					}
					else if (side == Chirality.Right)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Right, "ButtonA", false));
					}
					if (both)
					{
						anyInput.Inputs.Add(InputNode.Digital((side == Chirality.Left) ? Chirality.Right : Chirality.Left, "ButtonA", false));
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(TouchController), "Bind")]
	class TouchControllerBindPatch
	{
		[HarmonyPostfix]
		static void Postfix(InputGroup group, TouchController __instance)
		{
			Chirality? side = group.Side;
			Chirality side2 = __instance.Side;
			if (!(side.GetValueOrDefault() == side2 & side != null))
			{
				return;
			}

			// Captures controller objects for later use.
			if (leftController == null && side2 == Chirality.Left)
			{
				leftController = __instance;
			}
			else if (rightController == null && side2 == Chirality.Right)
			{
				rightController = __instance;
			}

			CommonToolInputs commonToolInputs = group as CommonToolInputs;
			if (commonToolInputs != null)
			{
				// Clears bindings from the vanilla method
				commonToolInputs.ClearBindings();
				commonToolInputs.Interact.AddBinding(__instance.TriggerClick);
				commonToolInputs.Secondary.AddBinding(__instance.JoystickClick);
				commonToolInputs.Grab.AddBinding(__instance.GripClick);
				commonToolInputs.Menu.AddBinding(__instance.ButtonYB);
				commonToolInputs.Strength.AddBinding(__instance.Trigger);
				commonToolInputs.Axis.AddBinding(__instance.Joystick);

				// Removing this binding will activate one button mode for whatever Menu is bound to.
				// commonToolInputs.UserspaceToggle.AddBinding(InputNode.Digital(this.ButtonXA), this, null, 0);
			}
		}
	}

	[HarmonyPatch(typeof(TouchController), "BindNodeActions")]
	class TouchControllersBindNodeActionsPatch
	{
		[HarmonyPrefix]
		public static void Postfix(IInputNode node, string name = null)
		{
			if (!(config.GetValue(LeftHand) == Setting.Jump) && !(config.GetValue(RightHand) == Setting.Jump)) return;
			bool both = (config.GetValue(LeftHand) == Setting.Jump) && (config.GetValue(RightHand) == Setting.Jump);
			Chirality side = (config.GetValue(LeftHand) == Setting.Jump) ? Chirality.Left : Chirality.Right;


			IDualAxisInputNode dualAxisInputNode = node as IDualAxisInputNode;
			if (dualAxisInputNode == null)
			{
				AnyInput anyInput = node as AnyInput;
				if (anyInput != null && name == "Jump")
				{
					if (side == Chirality.Left)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Left, "ButtonXA", false));
					}
					else if (side == Chirality.Right)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Right, "ButtonXA", false));
					}
					if (both)
					{
						anyInput.Inputs.Add(InputNode.Digital((side == Chirality.Left) ? Chirality.Right : Chirality.Left, "ButtonXA", false));
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(HPReverbController), "Bind")]
	class HPReverbControllerBindPatch
	{
		[HarmonyPostfix]
		static void Postfix(InputGroup group, HPReverbController __instance)
		{
			Chirality? side = group.Side;
			Chirality side2 = __instance.Side;
			if (!(side.GetValueOrDefault() == side2 & side != null))
			{
				return;
			}

			// Captures controller objects for later use.
			if (leftController == null && side2 == Chirality.Left)
			{
				leftController = __instance;
			}
			else if (rightController == null && side2 == Chirality.Right)
			{
				rightController = __instance;
			}

			CommonToolInputs commonToolInputs = group as CommonToolInputs;
			if (commonToolInputs != null)
			{
				// Clears bindings from the vanilla method
				commonToolInputs.ClearBindings();
				commonToolInputs.Interact.AddBinding(__instance.TriggerClick);
				commonToolInputs.Secondary.AddBinding(__instance.JoystickClick);
				commonToolInputs.Grab.AddBinding(__instance.GripClick);
				commonToolInputs.Menu.AddBinding(__instance.ButtonYB);
				commonToolInputs.Strength.AddBinding(__instance.Trigger);
				commonToolInputs.Axis.AddBinding(__instance.Joystick);

				// Removing this binding will activate one button mode for whatever Menu is bound to.
				// commonToolInputs.UserspaceToggle.AddBinding(InputNode.Digital(this.ButtonXA), this, null, 0);
			}
		}
	}

	[HarmonyPatch(typeof(HPReverbController), "BindNodeActions")]
	class HPReverbControllersBindNodeActionsPatch
	{
		[HarmonyPrefix]
		public static void Postfix(IInputNode node, string name = null)
		{
			if (!(config.GetValue(LeftHand) == Setting.Jump) && !(config.GetValue(RightHand) == Setting.Jump)) return;
			bool both = (config.GetValue(LeftHand) == Setting.Jump) && (config.GetValue(RightHand) == Setting.Jump);
			Chirality side = (config.GetValue(LeftHand) == Setting.Jump) ? Chirality.Left : Chirality.Right;


			IDualAxisInputNode dualAxisInputNode = node as IDualAxisInputNode;
			if (dualAxisInputNode == null)
			{
				AnyInput anyInput = node as AnyInput;
				if (anyInput != null && name == "Jump")
				{
					if (side == Chirality.Left)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Left, "ButtonXA", false));
					}
					else if (side == Chirality.Right)
					{
						anyInput.Inputs.Add(InputNode.Digital(Chirality.Right, "ButtonXA", false));
					}
					if (both)
					{
						anyInput.Inputs.Add(InputNode.Digital((side == Chirality.Left) ? Chirality.Right : Chirality.Left, "ButtonXA", false));
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(DualControllerBindingGenerator), "Bind")]
	class DualControllerBindingGeneratorBindPatch
	{
		// Captures the InputGroups neccessary to rebind various bindings.
		public static void Prefix(InputGroup group)
		{
			if (dualControllerGlobalActionsInputGroup == null && group is GlobalActions)
			{
				dualControllerGlobalActionsInputGroup = group;
			}
			else if (dualControllerSmoothLocomotionInputGroup == null && group is SmoothLocomotionInputs)
			{
				dualControllerSmoothLocomotionInputGroup = group;
			}
			else if (dualControllerSmooth3AxisLocomotionInputGroup == null && group is SmoothThreeAxisLocomotionInputs)
			{
				dualControllerSmooth3AxisLocomotionInputGroup = group;
			}
			else if (dualControllerPhotoActionsInputGroup == null && group is PhotoInputs)
			{
				dualControllerPhotoActionsInputGroup = group;
			}
			else if (dualControllerUndoInputGroup == null && group is FrooxEngine.Undo.UndoInputs)
			{
				dualControllerUndoInputGroup = group;
			}
		}
	}

	[HarmonyPatch(typeof(DualControllerBindingGenerator), "Bind")]
	class DualControllerBindingGeneratorBindPatch2
	{
		public static void Postfix()
		{
			ControllerType leftType = ControllerType.None, rightType = ControllerType.None;
			TrackedObject leftControllerCast = null, rightControllerCast = null;

			switch (leftController)
			{
				case IndexController indexController:
					leftControllerCast = leftController as IndexController;
					leftType = ControllerType.Index;
					break;
				case TouchController touchController:
					leftControllerCast = leftController as TouchController;
					leftType = ControllerType.Touch;
					break;
				case HPReverbController hPReverbController:
					leftControllerCast = leftController as HPReverbController;
					leftType = ControllerType.Reverb;
					break;
			}

			switch (rightController)
			{
				case IndexController indexController:
					rightControllerCast = rightController as IndexController;
					rightType = ControllerType.Index;
					break;
				case TouchController touchController:
					rightControllerCast = rightController as TouchController;
					rightType = ControllerType.Touch;
					break;
				case HPReverbController hPReverbController:
					rightControllerCast = rightController as HPReverbController;
					rightType = ControllerType.Reverb;
					break;
			}

			if (leftControllerCast == null && rightControllerCast == null) return;

			if (dualControllerGlobalActionsInputGroup != null)
			{
				GlobalActions globalActions = dualControllerGlobalActionsInputGroup as GlobalActions;
				globalActions.ToggleMute.ClearBindings();
				if (config.GetValue(LeftHand) == Setting.MuteToggle && leftControllerCast != null)
				{
					switch (leftType)
					{
						case ControllerType.Index:
							IndexController indexController = leftControllerCast as IndexController;
							globalActions.ToggleMute.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = leftControllerCast as TouchController;
							globalActions.ToggleMute.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = leftControllerCast as HPReverbController;
							globalActions.ToggleMute.AddBinding(hPReverbController.ButtonXA);
							break;
						case ControllerType.None:
							break;
					}
				}

				if (config.GetValue(RightHand) == Setting.MuteToggle && rightControllerCast != null)
				{
					switch (rightType)
					{
						case ControllerType.Index:
							IndexController indexController = rightControllerCast as IndexController;
							globalActions.ToggleMute.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = rightControllerCast as TouchController;
							globalActions.ToggleMute.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = rightControllerCast as HPReverbController;
							globalActions.ToggleMute.AddBinding(hPReverbController.ButtonXA);
							break;
						case ControllerType.None:
							break;
					}
				}
			}
		}
	}
	
	// Currently unused
	[HarmonyPatch(typeof(KeyboardAndMouseBindingGenerator), "Bind")]
	class KeyboardAndMouseBindingGeneratorBindPatch
	{
		public static void Prefix(InputGroup group)
		{
			if (desktopPhotoActionsInputGroup == null && group is PhotoInputs)
			{
				desktopPhotoActionsInputGroup = group;
			}
			if (desktopUndoInputGroup == null && group is FrooxEngine.Undo.UndoInputs)
			{
				desktopUndoInputGroup = group;
			}
		}
	}

	// Currently unused
	[HarmonyPatch(typeof(KeyboardAndMouseBindingGenerator), "Bind")]
	class KeyboardAndMouseBindingGeneratorBindPatch2
	{
		public static void Postfix(DualControllerBindingGenerator __instance, InputGroup group)
		{
			ControllerType leftType = ControllerType.None, rightType = ControllerType.None;
			TrackedObject leftControllerCast = null, rightControllerCast = null;

			switch (leftController)
			{
				case IndexController indexController:
					leftControllerCast = leftController as IndexController;
					leftType = ControllerType.Index;
					break;
				case TouchController touchController:
					leftControllerCast = leftController as TouchController;
					leftType = ControllerType.Touch;
					break;
				case HPReverbController hPReverbController:
					leftControllerCast = leftController as HPReverbController;
					leftType = ControllerType.Reverb;
					break;
			}

			switch (rightController)
			{
				case IndexController indexController:
					rightControllerCast = rightController as IndexController;
					rightType = ControllerType.Index;
					break;
				case TouchController touchController:
					rightControllerCast = rightController as TouchController;
					rightType = ControllerType.Touch;
					break;
				case HPReverbController hPReverbController:
					rightControllerCast = rightController as HPReverbController;
					rightType = ControllerType.Reverb;
					break;
			}

			if (leftControllerCast == null && rightControllerCast == null) return;

			if (desktopPhotoActionsInputGroup != null)
			{
				PhotoInputs photoInputs = desktopPhotoActionsInputGroup as PhotoInputs;
				if (config.GetValue(LeftHand) == Setting.Photo && leftControllerCast != null)
				{
					photoInputs.TakePhoto.ClearBindings();
					switch (leftType)
					{
						case ControllerType.Index:
							IndexController indexController = leftControllerCast as IndexController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = leftControllerCast as TouchController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = leftControllerCast as HPReverbController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(hPReverbController.ButtonXA);
							break;

					}
				}
				if (config.GetValue(RightHand) == Setting.Photo && rightControllerCast != null)
				{
					switch (rightType)
					{
						case ControllerType.Index:
							IndexController indexController = rightControllerCast as IndexController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = rightControllerCast as TouchController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = rightControllerCast as HPReverbController;
							photoInputs.TakePhoto.ClearBindings();
							photoInputs.TakePhoto.AddBinding(hPReverbController.ButtonXA);
							break;
					}
				}
			}

			if (desktopUndoInputGroup != null)
			{
				FrooxEngine.Undo.UndoInputs undoInputs = dualControllerUndoInputGroup as FrooxEngine.Undo.UndoInputs;
				if (config.GetValue(LeftHand) == Setting.Undo && leftControllerCast != null)
				{
					undoInputs.Undo.ClearBindings();
					switch (leftType)
					{
						case ControllerType.Index:
							IndexController indexController = leftControllerCast as IndexController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = leftControllerCast as TouchController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = leftControllerCast as HPReverbController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(hPReverbController.ButtonXA);
							break;
					}
				}
				if (config.GetValue(RightHand) == Setting.Undo && rightControllerCast != null)
				{
					switch (rightType)
					{
						case ControllerType.Index:
							IndexController indexController = rightControllerCast as IndexController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(indexController.ButtonA);
							break;
						case ControllerType.Touch:
							TouchController touchController = rightControllerCast as TouchController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(touchController.ButtonXA);
							break;
						case ControllerType.Reverb:
							HPReverbController hPReverbController = rightControllerCast as HPReverbController;
							undoInputs.Undo.ClearBindings();
							undoInputs.Undo.AddBinding(hPReverbController.ButtonXA);
							break;
					}
				}
			}
		}
	}

}