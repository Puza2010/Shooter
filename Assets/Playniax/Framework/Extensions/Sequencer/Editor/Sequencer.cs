using UnityEditor;
using UnityEngine;
using Playniax.Framework.Editor;

namespace Playniax.Sequencer.Editor
{
    public class Helpers : EditorWindowHelpers
    {
        [MenuItem("Tools/Playniax/Sequencer/Sequencer", false, 81)]
        public static SequencerBase Add_Sequencer()
        {
            var sequencer = new GameObject("Sequencer").AddComponent<SequencerBase>();

            Undo.RegisterCreatedObjectUndo(sequencer.gameObject, "Create object");

            Selection.activeGameObject = sequencer.gameObject;

            return sequencer;
        }

        [MenuItem("Tools/Playniax/Sequencer/Message", false, 81)]
        public static void Add_Sequence_Message()
        {
            var sequencer = _Get_Sequencer();
            if (sequencer == null) sequencer = Add_Sequencer();

            var message = new GameObject("Message", typeof(Message));
            message.transform.SetParent(sequencer.transform);

            Undo.RegisterCreatedObjectUndo(message.gameObject, "Create object");

            Selection.activeGameObject = message.gameObject;
        }

        [MenuItem("Tools/Playniax/Sequencer/Sequence Spawner", false, 81)]
        public static void Add_Sequence_Spawner()
        {
            var sequencer = _Get_Sequencer();
            if (sequencer == null) sequencer = Add_Sequencer();

            var spawner = new GameObject("Sequence Spawner", typeof(SequenceSpawner));
            spawner.transform.SetParent(sequencer.transform);

            Undo.RegisterCreatedObjectUndo(spawner.gameObject, "Create object");

            Selection.activeGameObject = spawner.gameObject;
        }

        [MenuItem("Tools/Playniax/Sequencer/Wait For Sequence To Be Finished", false, 81)]
        public static void Add_Sequence_WaitForSequenceToBeFinished()
        {
            var sequencer = _Get_Sequencer();
            if (sequencer == null) sequencer = Add_Sequencer();

            var waitForSequenceToBeFinished = new GameObject("Wait For Sequence To Be Finished", typeof(WaitForSequenceToBeFinished));
            waitForSequenceToBeFinished.transform.SetParent(sequencer.transform);

            Undo.RegisterCreatedObjectUndo(waitForSequenceToBeFinished.gameObject, "Create object");

            Selection.activeGameObject = waitForSequenceToBeFinished.gameObject;
        }

        [MenuItem("Tools/Playniax/Sequencer/Container", false, 81)]
        public static void Add_Sequence_Container()
        {
            var sequencer = _Get_Sequencer();
            if (sequencer == null) sequencer = Add_Sequencer();

            var container = new GameObject("Container", typeof(Container));
            container.transform.SetParent(sequencer.transform);

            Undo.RegisterCreatedObjectUndo(container.gameObject, "Create object");

            Selection.activeGameObject = container.gameObject;
        }

        [MenuItem("Tools/Playniax/Sequencer/Delay", false, 81)]
        public static void Add_Delay()
        {
            var sequencer = _Get_Sequencer();
            if (sequencer == null) sequencer = Add_Sequencer();

            var delay = new GameObject("Delay", typeof(Delay));
            delay.transform.SetParent(sequencer.transform);

            Undo.RegisterCreatedObjectUndo(delay.gameObject, "Create object");

            Selection.activeGameObject = delay.gameObject;
        }
        static SequencerBase _Get_Sequencer()
        {
            SequencerBase sequencer;

            var all = Object.FindObjectsOfType<SequencerBase>();
            if (all.Length == 0) return null;

            var active = Selection.activeGameObject;

            if (active)
            {
                sequencer = active.GetComponent<SequencerBase>();
                if (sequencer) return sequencer;
            }

            if (all[0]) return all[0];

            return null;
        }
    }
}
