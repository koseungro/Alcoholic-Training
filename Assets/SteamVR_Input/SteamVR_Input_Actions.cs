//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Valve.VR
{
    using System;
    using UnityEngine;
    
    
    public partial class SteamVR_Actions
    {
        
        private static SteamVR_Action_Boolean p_outDoorFinish_hmd;
        
        private static SteamVR_Action_Boolean p_outDoorFinish_click;
        
        private static SteamVR_Action_Pose p_outDoorFinish_pose;
        
        private static SteamVR_Action_Vibration p_outDoorFinish_vibration;
        
        public static SteamVR_Action_Boolean outDoorFinish_hmd
        {
            get
            {
                return SteamVR_Actions.p_outDoorFinish_hmd.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Boolean outDoorFinish_click
        {
            get
            {
                return SteamVR_Actions.p_outDoorFinish_click.GetCopy<SteamVR_Action_Boolean>();
            }
        }
        
        public static SteamVR_Action_Pose outDoorFinish_pose
        {
            get
            {
                return SteamVR_Actions.p_outDoorFinish_pose.GetCopy<SteamVR_Action_Pose>();
            }
        }
        
        public static SteamVR_Action_Vibration outDoorFinish_vibration
        {
            get
            {
                return SteamVR_Actions.p_outDoorFinish_vibration.GetCopy<SteamVR_Action_Vibration>();
            }
        }
        
        private static void InitializeActionArrays()
        {
            Valve.VR.SteamVR_Input.actions = new Valve.VR.SteamVR_Action[] {
                    SteamVR_Actions.outDoorFinish_hmd,
                    SteamVR_Actions.outDoorFinish_click,
                    SteamVR_Actions.outDoorFinish_pose,
                    SteamVR_Actions.outDoorFinish_vibration};
            Valve.VR.SteamVR_Input.actionsIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.outDoorFinish_hmd,
                    SteamVR_Actions.outDoorFinish_click,
                    SteamVR_Actions.outDoorFinish_pose};
            Valve.VR.SteamVR_Input.actionsOut = new Valve.VR.ISteamVR_Action_Out[] {
                    SteamVR_Actions.outDoorFinish_vibration};
            Valve.VR.SteamVR_Input.actionsVibration = new Valve.VR.SteamVR_Action_Vibration[] {
                    SteamVR_Actions.outDoorFinish_vibration};
            Valve.VR.SteamVR_Input.actionsPose = new Valve.VR.SteamVR_Action_Pose[] {
                    SteamVR_Actions.outDoorFinish_pose};
            Valve.VR.SteamVR_Input.actionsBoolean = new Valve.VR.SteamVR_Action_Boolean[] {
                    SteamVR_Actions.outDoorFinish_hmd,
                    SteamVR_Actions.outDoorFinish_click};
            Valve.VR.SteamVR_Input.actionsSingle = new Valve.VR.SteamVR_Action_Single[0];
            Valve.VR.SteamVR_Input.actionsVector2 = new Valve.VR.SteamVR_Action_Vector2[0];
            Valve.VR.SteamVR_Input.actionsVector3 = new Valve.VR.SteamVR_Action_Vector3[0];
            Valve.VR.SteamVR_Input.actionsSkeleton = new Valve.VR.SteamVR_Action_Skeleton[0];
            Valve.VR.SteamVR_Input.actionsNonPoseNonSkeletonIn = new Valve.VR.ISteamVR_Action_In[] {
                    SteamVR_Actions.outDoorFinish_hmd,
                    SteamVR_Actions.outDoorFinish_click};
        }
        
        private static void PreInitActions()
        {
            SteamVR_Actions.p_outDoorFinish_hmd = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/OutDoorFinish/in/hmd")));
            SteamVR_Actions.p_outDoorFinish_click = ((SteamVR_Action_Boolean)(SteamVR_Action.Create<SteamVR_Action_Boolean>("/actions/OutDoorFinish/in/click")));
            SteamVR_Actions.p_outDoorFinish_pose = ((SteamVR_Action_Pose)(SteamVR_Action.Create<SteamVR_Action_Pose>("/actions/OutDoorFinish/in/pose")));
            SteamVR_Actions.p_outDoorFinish_vibration = ((SteamVR_Action_Vibration)(SteamVR_Action.Create<SteamVR_Action_Vibration>("/actions/OutDoorFinish/out/vibration")));
        }
    }
}
