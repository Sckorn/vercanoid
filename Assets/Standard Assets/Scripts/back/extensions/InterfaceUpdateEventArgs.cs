using UnityEngine;
using System;
using System.Collections;

public class InterfaceUpdateEventArgs : EventArgs
{

    private InterfaceUpdateReasons updateReason;

    public InterfaceUpdateReasons UpdateReason
    {
        get { return this.updateReason; }
    }

    private string updatedValue; //balls number, current score

    public string UpdatedValue
    {
        get { return this.updatedValue; }
    }

    private bool exceptionUpdate;

    public bool ExceptionUpdate
    {
        get { return this.exceptionUpdate; }
    }

    private Exception thrownException = null;

    public Exception ThrownException
    {
        get { return this.thrownException; }
    }

    public InterfaceUpdateEventArgs(InterfaceUpdateReasons updateReason, string updatedValue, Exception thrownException = null)
    {
        this.updateReason = updateReason;
        this.updatedValue = updatedValue;

        if (thrownException != null)
        {
            this.exceptionUpdate = true;
            this.thrownException = thrownException;
        }
    }

}
