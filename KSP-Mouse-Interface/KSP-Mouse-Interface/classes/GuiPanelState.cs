using System.Collections.Generic;
using UnityEngine;

namespace KSPMouseInterface
{
    public class GuiPanelState
    {
        private bool mOpenCloseCangeAllowed = true;
        private bool mOpenCloseDown = false;
        public bool OpenCloseToggled { get; set; }
        public bool OpenCloseDown
        {
            get
            {
                return mOpenCloseDown;
            }
            set
            {
                mOpenCloseDown = value;

                if (mOpenCloseCangeAllowed && value)
                {
                    OpenCloseToggled = !OpenCloseToggled;
                    mOpenCloseCangeAllowed = false;
                }

                if (!value)
                    mOpenCloseCangeAllowed = true;

            }
        }

        private bool mSettingsCangeAllowed = true;
        private bool mSettingsDown = false;
        public bool SettingsToggled { get; set; }
        public bool SettingsDown
        {
            get
            {
                return mSettingsDown;
            }
            set
            {
                mSettingsDown = value;

                if (mSettingsCangeAllowed && value)
                {
                    SettingsToggled = !SettingsToggled;
                    mSettingsCangeAllowed = false;
                }

                if (!value)
                    mSettingsCangeAllowed = true;

            }
        }

        public Directions Yaw { get; set; }
        public Directions Pitch { get; set; }
        public Directions Roll { get; set; }

        public Directions RCSYaw { get; set; }
        public Directions RCSPitch { get; set; }
        public Directions RCSRoll { get; set; }

        private bool mNextStageCangeAllowed = true;
        private bool mNextStageDown = false;
        public bool NextStageToggled { get; set; }
        public bool NextStageDown
        {
            get
            {
                return mNextStageDown;
            }
            set
            {
                mNextStageDown = value;

                if (mNextStageCangeAllowed && value)
                {
                    NextStageToggled = !NextStageToggled;
                    mNextStageCangeAllowed = false;
                }
                else if (NextStageToggled == value)
                {
                    NextStageToggled = false;
                }

                if (!value)
                    mNextStageCangeAllowed = true;

            }
        }

        public Directions Throttle { get; set; }
        public Directions ThrottleMax { get; set; }
        public bool ThrottleOff { get; set; }

        private bool mRCSCangeAllowed = true;
        private bool mRCSDown = false;
        public bool RCSToggled { get; set; }
        public bool RCSDown
        {
            get
            {
                return mRCSDown;
            }
            set
            {
                mRCSDown = value;

                if (mRCSCangeAllowed && value)
                {
                    RCSToggled = !RCSToggled;
                    mRCSCangeAllowed = false;
                }

                if (!value)
                    mRCSCangeAllowed = true;

            }
        }

        private bool mSASCangeAllowed = true;
        private bool mSASDown = false;
        public bool SASToggled { get; set; }
        public bool SASDown
        {
            get
            {
                return mSASDown;
            }
            set
            {
                mSASDown = value;

                if (mSASCangeAllowed && value)
                {
                    SASToggled = !SASToggled;
                    mSASCangeAllowed = false;
                }

                if (!value)
                    mSASCangeAllowed = true;

            }
        }

        #region ActionGroups

        private bool mActionGroup0CangeAllowed = true;
        private bool mActionGroup0Down = false;
        public bool ActionGroup0Toggled { get; set; }
        public bool ActionGroup0Down
        {
            get
            {
                return mActionGroup0Down;
            }
            set
            {
                mActionGroup0Down = value;

                if (mActionGroup0CangeAllowed && value)
                {
                    ActionGroup0Toggled = !ActionGroup0Toggled;
                    mActionGroup0CangeAllowed = false;
                }

                if (!value)
                    mActionGroup0CangeAllowed = true;

            }
        }

        private bool mActionGroup1CangeAllowed = true;
        private bool mActionGroup1Down = false;
        public bool ActionGroup1Toggled { get; set; }
        public bool ActionGroup1Down
        {
            get
            {
                return mActionGroup1Down;
            }
            set
            {
                mActionGroup1Down = value;

                if (mActionGroup1CangeAllowed && value)
                {
                    ActionGroup1Toggled = !ActionGroup1Toggled;
                    mActionGroup1CangeAllowed = false;
                }

                if (!value)
                    mActionGroup1CangeAllowed = true;

            }
        }

        private bool mActionGroup2CangeAllowed = true;
        private bool mActionGroup2Down = false;
        public bool ActionGroup2Toggled { get; set; }
        public bool ActionGroup2Down
        {
            get
            {
                return mActionGroup2Down;
            }
            set
            {
                mActionGroup2Down = value;

                if (mActionGroup2CangeAllowed && value)
                {
                    ActionGroup2Toggled = !ActionGroup2Toggled;
                    mActionGroup2CangeAllowed = false;
                }

                if (!value)
                    mActionGroup2CangeAllowed = true;

            }
        }

        private bool mActionGroup3CangeAllowed = true;
        private bool mActionGroup3Down = false;
        public bool ActionGroup3Toggled { get; set; }
        public bool ActionGroup3Down
        {
            get
            {
                return mActionGroup3Down;
            }
            set
            {
                mActionGroup3Down = value;

                if (mActionGroup3CangeAllowed && value)
                {
                    ActionGroup3Toggled = !ActionGroup3Toggled;
                    mActionGroup3CangeAllowed = false;
                }

                if (!value)
                    mActionGroup3CangeAllowed = true;

            }
        }

        private bool mActionGroup4CangeAllowed = true;
        private bool mActionGroup4Down = false;
        public bool ActionGroup4Toggled { get; set; }
        public bool ActionGroup4Down
        {
            get
            {
                return mActionGroup4Down;
            }
            set
            {
                mActionGroup4Down = value;

                if (mActionGroup4CangeAllowed && value)
                {
                    ActionGroup4Toggled = !ActionGroup4Toggled;
                    mActionGroup4CangeAllowed = false;
                }

                if (!value)
                    mActionGroup4CangeAllowed = true;

            }
        }

        private bool mActionGroup5CangeAllowed = true;
        private bool mActionGroup5Down = false;
        public bool ActionGroup5Toggled { get; set; }
        public bool ActionGroup5Down
        {
            get
            {
                return mActionGroup5Down;
            }
            set
            {
                mActionGroup5Down = value;

                if (mActionGroup5CangeAllowed && value)
                {
                    ActionGroup5Toggled = !ActionGroup5Toggled;
                    mActionGroup5CangeAllowed = false;
                }

                if (!value)
                    mActionGroup5CangeAllowed = true;

            }
        }

        private bool mActionGroup6CangeAllowed = true;
        private bool mActionGroup6Down = false;
        public bool ActionGroup6Toggled { get; set; }
        public bool ActionGroup6Down
        {
            get
            {
                return mActionGroup6Down;
            }
            set
            {
                mActionGroup6Down = value;

                if (mActionGroup6CangeAllowed && value)
                {
                    ActionGroup6Toggled = !ActionGroup6Toggled;
                    mActionGroup6CangeAllowed = false;
                }

                if (!value)
                    mActionGroup6CangeAllowed = true;

            }
        }

        private bool mActionGroup7CangeAllowed = true;
        private bool mActionGroup7Down = false;
        public bool ActionGroup7Toggled { get; set; }
        public bool ActionGroup7Down
        {
            get
            {
                return mActionGroup7Down;
            }
            set
            {
                mActionGroup7Down = value;

                if (mActionGroup7CangeAllowed && value)
                {
                    ActionGroup7Toggled = !ActionGroup7Toggled;
                    mActionGroup7CangeAllowed = false;
                }

                if (!value)
                    mActionGroup7CangeAllowed = true;

            }
        }

        private bool mActionGroup8CangeAllowed = true;
        private bool mActionGroup8Down = false;
        public bool ActionGroup8Toggled { get; set; }
        public bool ActionGroup8Down
        {
            get
            {
                return mActionGroup8Down;
            }
            set
            {
                mActionGroup8Down = value;

                if (mActionGroup8CangeAllowed && value)
                {
                    ActionGroup8Toggled = !ActionGroup8Toggled;
                    mActionGroup8CangeAllowed = false;
                }

                if (!value)
                    mActionGroup8CangeAllowed = true;

            }
        }

        private bool mActionGroup9CangeAllowed = true;
        private bool mActionGroup9Down = false;
        public bool ActionGroup9Toggled { get; set; }
        public bool ActionGroup9Down
        {
            get
            {
                return mActionGroup9Down;
            }
            set
            {
                mActionGroup9Down = value;

                if (mActionGroup9CangeAllowed && value)
                {
                    ActionGroup9Toggled = !ActionGroup9Toggled;
                    mActionGroup9CangeAllowed = false;
                }

                if (!value)
                    mActionGroup9CangeAllowed = true;

            }
        }

        #endregion

        #region Func

        private bool mFunc0CangeAllowed = true;
        private bool mFunc0Down = false;
        public bool Func0Toggled { get; set; }
        public bool Func0Down
        {
            get
            {
                return mFunc0Down;
            }
            set
            {
                mFunc0Down = value;

                if (mFunc0CangeAllowed && value)
                {
                    Func0Toggled = !ActionGroup9Toggled;
                    mFunc0CangeAllowed = false;
                }

                if (!value)
                    mFunc0CangeAllowed = true;

            }
        }

        private bool mFunc1CangeAllowed = true;
        private bool mFunc1Down = false;
        public bool Func1Toggled { get; set; }
        public bool Func1Down
        {
            get
            {
                return mFunc1Down;
            }
            set
            {
                mFunc1Down = value;

                if (mFunc1CangeAllowed && value)
                {
                    Func1Toggled = !ActionGroup9Toggled;
                    mFunc1CangeAllowed = false;
                }

                if (!value)
                    mFunc1CangeAllowed = true;

            }
        }

        private bool mFunc2CangeAllowed = true;
        private bool mFunc2Down = false;
        public bool Func2Toggled { get; set; }
        public bool Func2Down
        {
            get
            {
                return mFunc2Down;
            }
            set
            {
                mFunc2Down = value;

                if (mFunc2CangeAllowed && value)
                {
                    Func2Toggled = !ActionGroup9Toggled;
                    mFunc2CangeAllowed = false;
                }

                if (!value)
                    mFunc2CangeAllowed = true;

            }
        }

        private bool mFunc3CangeAllowed = true;
        private bool mFunc3Down = false;
        public bool Func3Toggled { get; set; }
        public bool Func3Down
        {
            get
            {
                return mFunc3Down;
            }
            set
            {
                mFunc3Down = value;

                if (mFunc3CangeAllowed && value)
                {
                    Func3Toggled = !ActionGroup9Toggled;
                    mFunc3CangeAllowed = false;
                }

                if (!value)
                    mFunc3CangeAllowed = true;

            }
        }

        #endregion

        private Color mDefaultColor = new Color(0, 0, 0, 0);

        public Color Color { get; set; }
        public Vector2 TexCoords { get; set; }

        public bool AnalogInput { get; set; }
        public Vector2 AnalogInputValue { get; set; }
        public bool RCSAnalogInput { get; set; }
        public bool HasInput
        {
            get
            {
                return (Yaw != Directions.None || Pitch != Directions.None || Roll != Directions.None);
            }
        }


        public List<string> PropertyAsStrings
        {
            get
            {
                List<string> propertyStrings = new List<string>();
                propertyStrings.Add("HasInput = " + this.HasInput.ToString());
                propertyStrings.Add("AnalogInput = " + this.AnalogInput.ToString());
                propertyStrings.Add("RCSAnalogInput = " + this.AnalogInput.ToString());

                propertyStrings.Add("OpenCloseDown = " + this.OpenCloseDown.ToString());
                propertyStrings.Add("OpenCloseToggled = " + this.OpenCloseToggled.ToString());

                propertyStrings.Add("TexCoords = " + this.TexCoords.ToString());
                propertyStrings.Add("Color = " + this.Color.ToString());

                propertyStrings.Add("Pitch = " + this.Pitch.ToString());
                propertyStrings.Add("Yaw = " + this.Yaw.ToString());
                propertyStrings.Add("Roll = " + this.Roll.ToString());
                propertyStrings.Add("RCSPitch = " + this.RCSPitch.ToString());
                propertyStrings.Add("RCSYaw = " + this.RCSYaw.ToString());
                propertyStrings.Add("RCSRoll = " + this.RCSRoll.ToString());

                propertyStrings.Add("Throttle = " + this.Throttle.ToString());
                propertyStrings.Add("ThrottleMax = " + this.ThrottleMax.ToString());
                propertyStrings.Add("ThrottleOff = " + this.ThrottleOff.ToString());

                propertyStrings.Add("NextStageDown = " + this.NextStageDown.ToString());
                propertyStrings.Add("NextStageToggled = " + this.NextStageToggled.ToString());

                propertyStrings.Add("RCSDown = " + this.RCSDown.ToString());
                propertyStrings.Add("RCSToggled = " + this.RCSToggled.ToString());
                propertyStrings.Add("SASDown = " + this.SASDown.ToString());
                propertyStrings.Add("SASToggled = " + this.SASToggled.ToString());

                propertyStrings.Add("Func0Down = " + this.Func0Down.ToString());
                propertyStrings.Add("Func0Toggled = " + this.Func0Toggled.ToString());
                propertyStrings.Add("Func1Down = " + this.Func1Down.ToString());
                propertyStrings.Add("Func1Toggled = " + this.Func1Toggled.ToString());
                propertyStrings.Add("Func2Down = " + this.Func2Down.ToString());
                propertyStrings.Add("Func2Toggled = " + this.Func2Toggled.ToString());
                propertyStrings.Add("Func3Down = " + this.Func3Down.ToString());
                propertyStrings.Add("Func3Toggled = " + this.Func3Toggled.ToString());

                propertyStrings.Add("ActionGroup0Down = " + this.ActionGroup0Down.ToString());
                propertyStrings.Add("ActionGroup0Toggled = " + this.ActionGroup0Toggled.ToString());
                propertyStrings.Add("ActionGroup1Down = " + this.ActionGroup1Down.ToString());
                propertyStrings.Add("ActionGroup1Toggled = " + this.ActionGroup1Toggled.ToString());
                propertyStrings.Add("ActionGroup2Down = " + this.ActionGroup2Down.ToString());
                propertyStrings.Add("ActionGroup2Toggled = " + this.ActionGroup2Toggled.ToString());
                propertyStrings.Add("ActionGroup3Down = " + this.ActionGroup3Down.ToString());
                propertyStrings.Add("ActionGroup3Toggled = " + this.ActionGroup3Toggled.ToString());
                propertyStrings.Add("ActionGroup4Down = " + this.ActionGroup4Down.ToString());
                propertyStrings.Add("ActionGroup4Toggled = " + this.ActionGroup4Toggled.ToString());
                propertyStrings.Add("ActionGroup5Down = " + this.ActionGroup5Down.ToString());
                propertyStrings.Add("ActionGroup5Toggled = " + this.ActionGroup5Toggled.ToString());
                propertyStrings.Add("ActionGroup6Down = " + this.ActionGroup6Down.ToString());
                propertyStrings.Add("ActionGroup6Toggled = " + this.ActionGroup6Toggled.ToString());
                propertyStrings.Add("ActionGroup7Down = " + this.ActionGroup7Down.ToString());
                propertyStrings.Add("ActionGroup7Toggled = " + this.ActionGroup7Toggled.ToString());
                propertyStrings.Add("ActionGroup8Down = " + this.ActionGroup8Down.ToString());
                propertyStrings.Add("ActionGroup8Toggled = " + this.ActionGroup8Toggled.ToString());
                propertyStrings.Add("ActionGroup9Down = " + this.ActionGroup9Down.ToString());
                propertyStrings.Add("ActionGroup9Toggled = " + this.ActionGroup9Toggled.ToString());

                return propertyStrings;
            }
        }


        public GuiPanelState()
        {
            Reset();
        }


        public void Reset()
        {
            OpenCloseDown = false;
            OpenCloseToggled = false;

            Yaw = Directions.None;
            Pitch = Directions.None;
            Roll = Directions.None;
            RCSYaw = Directions.None;
            RCSPitch = Directions.None;
            RCSRoll = Directions.None;

            NextStageDown = false;
            NextStageToggled = false;

            Throttle = Directions.None;
            ThrottleMax = Directions.None;
            ThrottleOff = false;

            RCSDown = false;
            RCSToggled = false;
            SASDown = false;
            SASToggled = false;

            ActionGroup0Down = false;
            ActionGroup1Down = false;
            ActionGroup2Down = false;
            ActionGroup3Down = false;
            ActionGroup4Down = false;
            ActionGroup5Down = false;
            ActionGroup6Down = false;
            ActionGroup7Down = false;
            ActionGroup8Down = false;
            ActionGroup9Down = false;
            ActionGroup0Toggled = false;
            ActionGroup1Toggled = false;
            ActionGroup2Toggled = false;
            ActionGroup3Toggled = false;
            ActionGroup4Toggled = false;
            ActionGroup5Toggled = false;
            ActionGroup6Toggled = false;
            ActionGroup7Toggled = false;
            ActionGroup8Toggled = false;
            ActionGroup9Toggled = false;

            Func0Down = false;
            Func0Toggled = false;
            Func1Down = false;
            Func1Toggled = false;
            Func2Down = false;
            Func2Toggled = false;
            Func3Down = false;
            Func3Toggled = false;

            Color = mDefaultColor;

            AnalogInput = false;
            RCSAnalogInput = false;
        }
    }
 }
