using System;
using XNode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateNodeMenu("Dialog/Dialog Node")]
public class DialogNode : Node
{
    [Serializable]
    public sealed class Connection
    {
    }

    public const string InputPortName = "input";
    public const string OutputPortName = "output";

    [Input(ShowBackingValue.Never, ConnectionType.Multiple, TypeConstraint.Strict)]
    [FormerlySerializedAs("Entrada")]
    public Connection input;

    [Min(0.01f)]
    [Tooltip("Characters revealed per second.")]
    [FormerlySerializedAs("velocidadDeDisplay")]
    public float displaySpeed = 30f;

    [FormerlySerializedAs("eventoDeEntrada")]
    public UnityEvent entryEvent = new UnityEvent();

    [Min(0f)]
    [FormerlySerializedAs("esperaEventoDeEntrada")]
    public float entryEventWait;

    [Tooltip("If enabled, the message starts displaying while the entry event is still running.")]
    [FormerlySerializedAs("mostrarMensajeMientrasEventoDeEntradaSeEjecuta")]
    public bool displayMessageWhileEntryEventRuns;

    [TextArea(2, 8)]
    [FormerlySerializedAs("mensaje")]
    public string message;

    [FormerlySerializedAs("eventoDeSalida")]
    public UnityEvent exitEvent = new UnityEvent();

    [Min(0f)]
    [FormerlySerializedAs("esperaEventoDeSalida")]
    public float exitEventWait;

    [Output(ShowBackingValue.Never, ConnectionType.Override, TypeConstraint.Strict)]
    [FormerlySerializedAs("Salida")]
    public Connection output;

    public DialogNode GetNextNode()
    {
        NodePort outputPort = GetOutputPort(OutputPortName);
        if (outputPort == null || !outputPort.IsConnected)
        {
            return null;
        }

        return outputPort.Connection.node as DialogNode;
    }

    public override object GetValue(NodePort port)
    {
        return null;
    }
}
