using UnityEngine;
using System;
using System.Collections;

public class InterfaceUpdateEventArgs : EventArgs {

    private InterfaceUpdateReasons updateReason;

    public InterfaceUpdateReasons UpdateReason
    {
        get { return this.updateReason; }
    }

    private string updatedValue;

    public string UpdatedValue
    {
        get { return this.updatedValue; }
    }

    public InterfaceUpdateEventArgs(InterfaceUpdateReasons updateReason, string updatedValue)
    {
        this.updateReason = updateReason;
        this.updatedValue = updatedValue;
    }
	
}
