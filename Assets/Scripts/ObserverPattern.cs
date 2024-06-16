using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public void UpdateState();
}

public interface ISubject
{
    public void AddObserver(IObserver observer);
    public void RemoveObserver(IObserver observer);
    public void Notify();
}
