using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControler : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset 
    { 
        get; 
    }

    public @PlayerControler()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Controler"",
    ""maps"": [
        {
            ""name"": ""Character controller"",
            ""id"": ""03a06214-8aa7-4878-bc00-46785c36ea32"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""282a504d-0d9f-4bb1-935c-7bf2253cf49a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""63cbedeb-b0ee-49e5-86c2-21d88052eebe"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": ""NormalizeVector2"",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a9c80516-81ee-453a-93e8-038583b7713d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8b1680c6-3b12-4a2b-9f7a-c9f45718c081"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8e95e569-0ae8-4302-94a6-139f675f0af0"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ac286865-5091-455a-b97b-e7b581190ff6"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows"",
                    ""id"": ""25346ea3-9190-4516-9ec7-348979f96996"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ce9cf4ea-4887-4f22-9d43-62334f189457"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f1363468-bd60-449e-bc7f-9ec18799e7ea"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""b7aa1386-e820-41b0-b063-0feea257d579"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a7713e7d-aca2-44c2-a3de-f12b1dcb4201"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4c383c8d-ae02-443f-bf22-c6b83ba3f0d8"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""61f4ad64-aedb-48f1-9368-89aa9c415d93"",
            ""actions"": [
                {
                    ""name"": ""Navigate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""76ac0a42-6205-4467-88c9-1212f58dca6c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""0718ceb5-043f-4c22-b07f-32d811c5721d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""48e059b1-df5c-42c8-bbf0-61078ccb38c8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""020ffcd7-551e-4fee-aa9d-076735133725"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d142d6ba-433c-4c34-ad5c-2cd1f29bafd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""46be52c4-ef82-4dfe-b0e1-71f53e8e74bc"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""69a42968-8622-4f0e-908d-0c12b93fbed4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""74394fb2-63c2-4dfe-a460-c1cff4476b7b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TrackedDevicePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""30d9e6c2-7c3c-4ee2-93ad-a8fcd22a432d"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d4a62e8e-3763-4414-8622-807bc344cf11"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Gamepad"",
                    ""id"": ""6f5454eb-030c-4ed4-9e1d-9166cfd67135"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6c41889f-d9b9-4135-a9bf-68a74222ae1e"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e0b4f61b-1c4e-4900-986f-6336da83ed3b"",
                    ""path"": ""<Gamepad>/rightStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""803b8684-35d7-49b7-bb59-090a3bf52595"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e12b446e-fa35-4c8a-97a0-b2c8e1ca70df"",
                    ""path"": ""<Gamepad>/rightStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""64d06ba3-3d46-4297-8f24-1ffa700b5bfa"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9da7c617-3ff3-4f6b-972e-dcf6054ec7d9"",
                    ""path"": ""<Gamepad>/rightStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""300cb53f-686c-4986-8884-29c493af8b35"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8c8511e0-10a8-48de-a0a2-554fbfba3e39"",
                    ""path"": ""<Gamepad>/rightStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""97f13140-6d04-4b97-941d-7cc2a5ae0fe8"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Joystick"",
                    ""id"": ""96bce27a-29b0-48e3-b265-49e0a1f47df3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d7417fe7-c672-41e7-86d1-c06c1d631112"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ae3da729-f17d-42b8-96cf-7408d36f440e"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8f502632-53aa-4149-8a47-4f639c0078d7"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9f06f702-015e-4b4b-8ca8-4c56bc677ccd"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""465d7416-806c-4156-b7c4-a8973350a60a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ea63ab43-5cdb-4d10-9a13-eb3a70f3a261"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""79222d4f-31ea-42b0-9b8d-346898f8c083"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2926876a-badf-428c-97af-40c727d7db26"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ccb26d03-53bc-4187-9d9e-af202d9e64a8"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""46078268-beb7-49c6-b8fd-9dfb9aba4a83"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""70524943-de0e-47c0-9ac1-7b40082b76ba"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""77976576-8969-4788-a457-ba828a5d0d13"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""592d4fd1-252b-47dc-81fa-6e4b767d1f44"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""d99766f5-630c-43ea-9f49-285ae4452a43"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1884b07-05e8-4c7b-ad1a-1d7f01aea18c"",
                    ""path"": ""<Keyboard>/numpadEnter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ab6da6e0-418f-449c-9dc0-da1ae070e095"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6ee44d0-6dfb-47b7-a645-2f674a63af9f"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de7b5d6f-919c-407e-bdc5-6077888aebc0"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c8498f2-09a7-460b-8204-5c88cb4240a3"",
                    ""path"": ""<Touchscreen>/touch*/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d53704f-cadd-408f-b3f7-cc9bff286ce0"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8b82d76e-1179-4c7a-aa6b-dbe66969a8a2"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c18676e-ee7d-46c6-85b6-ee528dc58993"",
                    ""path"": ""<Touchscreen>/touch*/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a916ac7f-a2e0-4ae1-802d-9b20d155217f"",
                    ""path"": ""<XRController>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""27777022-6397-4d4f-a756-7f6149d124f3"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd69e7d0-2769-453c-99c6-68e3f0398e9f"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f6229c0-b4e5-48c3-8db9-3104973ed5ae"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43574b57-bd9d-4b73-b2cf-8348030e8a18"",
                    ""path"": ""<XRController>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDevicePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""829c1fa0-e8fb-4a07-87c8-daff34fd3470"",
                    ""path"": ""<XRController>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""XR"",
                    ""action"": ""TrackedDeviceOrientation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        m_Charactercontroller = asset.FindActionMap("Character controller", throwIfNotFound: true);

        m_Charactercontroller_Movement = m_Charactercontroller.FindAction("Movement", throwIfNotFound: true);

        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);

        m_UI_Navigate = m_UI.FindAction("Navigate", throwIfNotFound: true);

        m_UI_Submit = m_UI.FindAction("Submit", throwIfNotFound: true);
        
        m_UI_Cancel = m_UI.FindAction("Cancel", throwIfNotFound: true);
        
        m_UI_Point = m_UI.FindAction("Point", throwIfNotFound: true);
        
        m_UI_Click = m_UI.FindAction("Click", throwIfNotFound: true);
        
        m_UI_ScrollWheel = m_UI.FindAction("ScrollWheel", throwIfNotFound: true);
        
        m_UI_MiddleClick = m_UI.FindAction("MiddleClick", throwIfNotFound: true);
        
        m_UI_RightClick = m_UI.FindAction("RightClick", throwIfNotFound: true);
        
        m_UI_TrackedDevicePosition = m_UI.FindAction("TrackedDevicePosition", throwIfNotFound: true);
        
        m_UI_TrackedDeviceOrientation = m_UI.FindAction("TrackedDeviceOrientation", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;

        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;

        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    readonly InputActionMap m_Charactercontroller;
    
    ICharactercontrollerActions m_CharactercontrollerActionsCallbackInterface;
    
    readonly InputAction m_Charactercontroller_Movement;
    
    public struct CharactercontrollerActions
    {
        @PlayerControler m_Wrapper;
        
        public CharactercontrollerActions(@PlayerControler wrapper) { m_Wrapper = wrapper; }
        
        public InputAction @Movement => m_Wrapper.m_Charactercontroller_Movement;
        
        public InputActionMap Get() { return m_Wrapper.m_Charactercontroller; }
        
        public void Enable() { Get().Enable(); }
        
        public void Disable() { Get().Disable(); }
        
        public bool enabled => Get().enabled;
        
        public static implicit operator InputActionMap(CharactercontrollerActions set) { return set.Get(); }
        
        public void SetCallbacks(ICharactercontrollerActions instance)
        {
            if (m_Wrapper.m_CharactercontrollerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_CharactercontrollerActionsCallbackInterface.OnMovement;

                @Movement.performed -= m_Wrapper.m_CharactercontrollerActionsCallbackInterface.OnMovement;
                
                @Movement.canceled -= m_Wrapper.m_CharactercontrollerActionsCallbackInterface.OnMovement;
            }

            m_Wrapper.m_CharactercontrollerActionsCallbackInterface = instance;
            
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;

                @Movement.performed += instance.OnMovement;
                
                @Movement.canceled += instance.OnMovement;
            }
        }
    }
    public CharactercontrollerActions @Charactercontroller => new CharactercontrollerActions(this);

    readonly InputActionMap m_UI;
    
    IUIActions m_UIActionsCallbackInterface;
    
    readonly InputAction m_UI_Navigate;
    
    readonly InputAction m_UI_Submit;
    
    readonly InputAction m_UI_Cancel;
    
    readonly InputAction m_UI_Point;
    
    readonly InputAction m_UI_Click;
    
    readonly InputAction m_UI_ScrollWheel;
    
    readonly InputAction m_UI_MiddleClick;
    
    readonly InputAction m_UI_RightClick;
    
    readonly InputAction m_UI_TrackedDevicePosition;
    readonly InputAction m_UI_TrackedDeviceOrientation;
    
    public struct UIActions
    {
        @PlayerControler m_Wrapper;
        
        public UIActions(@PlayerControler wrapper) { m_Wrapper = wrapper; }
        
        public InputAction @Navigate => m_Wrapper.m_UI_Navigate;
        
        public InputAction @Submit => m_Wrapper.m_UI_Submit;
        
        public InputAction @Cancel => m_Wrapper.m_UI_Cancel;
        
        public InputAction @Point => m_Wrapper.m_UI_Point;
        
        public InputAction @Click => m_Wrapper.m_UI_Click;
        
        public InputAction @ScrollWheel => m_Wrapper.m_UI_ScrollWheel;
        
        public InputAction @MiddleClick => m_Wrapper.m_UI_MiddleClick;
        
        public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
        
        public InputAction @TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;
        
        public InputAction @TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;
        
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        
        public void Enable() { Get().Enable(); }
        
        public void Disable() { Get().Disable(); }
        
        public bool enabled => Get().enabled;
        
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Navigate.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;

                @Navigate.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                
                @Navigate.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                
                @Submit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                
                @Submit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                
                @Submit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                
                @Cancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                
                @Cancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                
                @Cancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                
                @Point.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                
                @Point.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                
                @Point.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                
                @Click.started -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                
                @Click.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                
                @Click.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                
                @ScrollWheel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                
                @ScrollWheel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                
                @ScrollWheel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                
                @MiddleClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                
                @MiddleClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                
                @MiddleClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                
                @RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                
                @RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                
                @RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                
                @TrackedDevicePosition.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                
                @TrackedDevicePosition.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                
                @TrackedDevicePosition.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                
                @TrackedDeviceOrientation.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                
                @TrackedDeviceOrientation.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                
                @TrackedDeviceOrientation.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
            }

            m_Wrapper.m_UIActionsCallbackInterface = instance;
            
            if (instance != null)
            {
                @Navigate.started += instance.OnNavigate;

                @Navigate.performed += instance.OnNavigate;
                
                @Navigate.canceled += instance.OnNavigate;
                
                @Submit.started += instance.OnSubmit;
                
                @Submit.performed += instance.OnSubmit;
                
                @Submit.canceled += instance.OnSubmit;
                
                @Cancel.started += instance.OnCancel;
                
                @Cancel.performed += instance.OnCancel;
                
                @Cancel.canceled += instance.OnCancel;
                
                @Point.started += instance.OnPoint;
                
                @Point.performed += instance.OnPoint;
                
                @Point.canceled += instance.OnPoint;
                
                @Click.started += instance.OnClick;
                
                @Click.performed += instance.OnClick;
                
                @Click.canceled += instance.OnClick;
                
                @ScrollWheel.started += instance.OnScrollWheel;
                
                @ScrollWheel.performed += instance.OnScrollWheel;
                
                @ScrollWheel.canceled += instance.OnScrollWheel;
                
                @MiddleClick.started += instance.OnMiddleClick;
                
                @MiddleClick.performed += instance.OnMiddleClick;
                
                @MiddleClick.canceled += instance.OnMiddleClick;
                
                @RightClick.started += instance.OnRightClick;
                
                @RightClick.performed += instance.OnRightClick;
                
                @RightClick.canceled += instance.OnRightClick;
                
                @TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
                
                @TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
                
                @TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
                
                @TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                
                @TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                
                @TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
            }
        }
    }

    public UIActions @UI => new UIActions(this);
    
    public interface ICharactercontrollerActions
    {
        void OnMovement(InputAction.CallbackContext context);
    }

    public interface IUIActions
    {
        void OnNavigate(InputAction.CallbackContext context);

        void OnSubmit(InputAction.CallbackContext context);
        
        void OnCancel(InputAction.CallbackContext context);
        
        void OnPoint(InputAction.CallbackContext context);
        
        void OnClick(InputAction.CallbackContext context);
        
        void OnScrollWheel(InputAction.CallbackContext context);
        
        void OnMiddleClick(InputAction.CallbackContext context);
        
        void OnRightClick(InputAction.CallbackContext context);
        
        void OnTrackedDevicePosition(InputAction.CallbackContext context);
        
        void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
    }
}