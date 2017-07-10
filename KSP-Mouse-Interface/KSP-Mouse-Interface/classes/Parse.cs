using UnityEngine;

namespace KSPMouseInterface
{
    public static class Parse
    {
        public static Margin MarginFrom(string marginAsString)
        {
            int l = 0;
            int r = 0;
            int t = 0;
            int b = 0;

            string[] temp = marginAsString.Split(',');

            if (temp == null || temp.Length != 4)
            {
                //print("KSPMouseInterface Error: Can't parse Margin");
            }
            else
            {
                try
                {
                    l = int.Parse(temp[0].Trim());
                    r = int.Parse(temp[1].Trim());
                    t = int.Parse(temp[2].Trim());
                    b = int.Parse(temp[3].Trim());
                }
                catch
                {
                    //print("KSPMouseInterface Error: Can't parse Margin");
                }
            }

            return new Margin(l, r, t, b);
        }

        public static Alignments AlignmentFrom(string alignmentAsString)
        {
            var alignment = Alignments.Bottom;

            switch (alignmentAsString)
            {
                case Consts.cLeft:
                    alignment = Alignments.Left;
                    break;
                case Consts.cRight:
                    alignment = Alignments.Right;
                    break;
                case Consts.cTop:
                    alignment = Alignments.Top;
                    break;
                case Consts.cBottom:
                    alignment = Alignments.Bottom;
                    break;
            }

            return alignment;
        }

        public static Color ColorFrom(string colorAsString)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            string[] temp = colorAsString.Split(',');

            if (temp == null || temp.Length != 3)
            {
                //print("KSPMouseInterface Error: Can't parse Color");
            }
            else
            {
                try
                {
                    r = int.Parse(temp[0].Trim());
                    g = int.Parse(temp[1].Trim());
                    b = int.Parse(temp[2].Trim());
                }
                catch
                {
                    //print("KSPMouseInterface Error: Can't parse Color");
                }
            }

            return new Color(r, g, b, 255);
        }

        public static Actions ActionFrom(string actionAsString)
        {
            var action = Actions.None;

            switch (actionAsString)
            {
                case Consts.cSettings:
                    action = Actions.Settings;
                    break;
                case Consts.cOpenClose:
                    action = Actions.OpenClose;
                    break;
                case Consts.cNextVessel:
                    action = Actions.NextVessel;
                    break;
                case Consts.cPrevVessel:
                    action = Actions.PrevVessel;
                    break;
                case Consts.cThrottleMax:
                    action = Actions.ThrottleMax;
                    break;
                case Consts.cThrottleUp:
                    action = Actions.ThrottleUp;
                    break;
                case Consts.cThrottleDown:
                    action = Actions.ThrottleDown;
                    break;
                case Consts.cThrottleOff:
                    action = Actions.ThrottleOff;
                    break;
                case Consts.cRCS:
                    action = Actions.RCS;
                    break;
                case Consts.cSAS:
                    action = Actions.SAS;
                    break;
                case Consts.cPitchUp:
                    action = Actions.PitchUp;
                    break;
                case Consts.cPitchDown:
                    action = Actions.PitchDown;
                    break;
                case Consts.cYawRight:
                    action = Actions.YawRight;
                    break;
                case Consts.cYawLeft:
                    action = Actions.YawLeft;
                    break;
                case Consts.cRollRight:
                    action = Actions.RollRight;
                    break;
                case Consts.cRollLeft:
                    action = Actions.RollLeft;
                    break;
                case Consts.cNextStage:
                    action = Actions.NextStage;
                    break;
                case Consts.cActiongroup0:
                    action = Actions.Actiongroup0;
                    break;
                case Consts.cActiongroup1:
                    action = Actions.Actiongroup1;
                    break;
                case Consts.cActiongroup2:
                    action = Actions.Actiongroup2;
                    break;
                case Consts.cActiongroup3:
                    action = Actions.Actiongroup3;
                    break;
                case Consts.cActiongroup4:
                    action = Actions.Actiongroup4;
                    break;
                case Consts.cActiongroup5:
                    action = Actions.Actiongroup5;
                    break;
                case Consts.cActiongroup6:
                    action = Actions.Actiongroup6;
                    break;
                case Consts.cActiongroup7:
                    action = Actions.Actiongroup7;
                    break;
                case Consts.cActiongroup8:
                    action = Actions.Actiongroup8;
                    break;
                case Consts.cActiongroup9:
                    action = Actions.Actiongroup9;
                    break;
                case Consts.cAnalogCenter:
                    action = Actions.AnalogCenter;
                    break;
                case Consts.cAnalogUp:
                    action = Actions.AnalogUp;
                    break;
                case Consts.cAnalogDown:
                    action = Actions.AnalogDown;
                    break;
                case Consts.cAnalogLeft:
                    action = Actions.AnalogLeft;
                    break;
                case Consts.cAnalogRight:
                    action = Actions.AnalogRight;
                    break;
                case Consts.cAnalogUpLeft:
                    action = Actions.AnalogUpLeft;
                    break;
                case Consts.cAnalogUpRight:
                    action = Actions.AnalogUpRight;
                    break;
                case Consts.cAnalogDownLeft:
                    action = Actions.AnalogDownLeft;
                    break;
                case Consts.cAnalogDownRight:
                    action = Actions.AnalogDownRight;
                    break;
                case Consts.cRCSPitchUp:
                    action = Actions.RCSPitchUp;
                    break;
                case Consts.cRCSPitchDown:
                    action = Actions.RCSPitchDown;
                    break;
                case Consts.cRCSYawRight:
                    action = Actions.RCSYawRight;
                    break;
                case Consts.cRCSYawLeft:
                    action = Actions.RCSYawLeft;
                    break;
                case Consts.cRCSRollRight:
                    action = Actions.RCSRollRight;
                    break;
                case Consts.cRCSRollLeft:
                    action = Actions.RCSRollLeft;
                    break;
                case Consts.cRCSAnalogCenter:
                    action = Actions.RCSAnalogCenter;
                    break;
                case Consts.cRCSAnalogUp:
                    action = Actions.RCSAnalogUp;
                    break;
                case Consts.cRCSAnalogDown:
                    action = Actions.RCSAnalogDown;
                    break;
                case Consts.cRCSAnalogLeft:
                    action = Actions.RCSAnalogLeft;
                    break;
                case Consts.cRCSAnalogRight:
                    action = Actions.RCSAnalogRight;
                    break;
                case Consts.cRCSAnalogUpLeft:
                    action = Actions.RCSAnalogUpLeft;
                    break;
                case Consts.cRCSAnalogUpRight:
                    action = Actions.RCSAnalogUpRight;
                    break;
                case Consts.cRCSAnalogDownLeft:
                    action = Actions.RCSAnalogDownLeft;
                    break;
                case Consts.cRCSAnalogDownRight:
                    action = Actions.RCSAnalogDownRight;
                    break;
                default:
                    action = Actions.None;
                    break;
            }

            return action;
        }

        public static ButtonActions ButtonActionFrom(string buttonActionAsString)
        {
            var action = ButtonActions.None;

            switch (buttonActionAsString)
            {
                case Consts.cClick:
                    action = ButtonActions.Click;
                    break;
                case Consts.cHover:
                    action = ButtonActions.Hover;
                    break;
            }

            return action;
        }

        public static int IntFrom(string intAsString)
        {
            int pos = 0;
            try
            {
                if (intAsString.Contains("%"))
                    pos = int.Parse(intAsString.Replace("%", "").Trim());
                else
                    pos = int.Parse(intAsString);
            }
            catch
            {
                //print("KSPMouseInterface error: Error during int parsing!");
            }

            return pos;
        }
    }
}