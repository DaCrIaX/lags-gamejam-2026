using XNode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/Dialog")]
public class Dialog : NodeGraph
{
    public DialogNode GetFirstNode()
    {
        foreach (Node node in nodes)
        {
            DialogNode dialogNode = node as DialogNode;
            if (dialogNode == null)
            {
                continue;
            }

            NodePort inputPort = dialogNode.GetInputPort(DialogNode.InputPortName);
            if (inputPort == null || !inputPort.IsConnected)
            {
                return dialogNode;
            }
        }

        return null;
    }
}
