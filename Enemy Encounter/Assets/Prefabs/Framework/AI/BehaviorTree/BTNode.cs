using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Check the status of the task (done/failed/in progress)(timer)
public enum NodeResult {
    Success,
    Failure,
    InProgress
}

public abstract class BTNode {


    private bool started = false;
    int priority; // Each node will have a priority, the lower the priority value, the higher the priority


    public int GetPriority()
    {
        return priority;
    }

    public virtual void SortPriority(ref int priorityCounter)
    {
        priority = priorityCounter++;
        Debug.Log($"{this} has priority {priority}");
    }

    public NodeResult UpdateNode() { // This gonna be called each frame
        // One-off thing task (just one step)
        if (!started) {
            started = true;
            NodeResult execResult = Execute(); // Execute the node and get the result
            if (execResult != NodeResult.InProgress) { // Tasks
                EndNode();
                return execResult;
            }
        }
        // Time-based tasks (multi-steps or based on conditions) - The Execution of the node is still in progress
        NodeResult updateResult = Update();
        if(updateResult != NodeResult.InProgress) // If after updated the node is finished hi execution(for example: Sequencer, Selector,Compositor,BlackboardDecorator)
        {
            EndNode();
        }
        return Update(); // for example: Sequencer, Selector,Compositor,BlackboardDecorator
    }

    // Functions to override in child classes
    // Execute the one thing task
    protected virtual NodeResult Execute() {
        return NodeResult.Success;
    }

    protected virtual NodeResult Update() {
        // Time-based tasks(tasks that takes time to be finished and just done instantly)
        return NodeResult.Success;
    }
    
    protected virtual void EndNode()
    {
        started = false;
        End();
    }

    protected virtual void End(){
        //clean up (reset)
    }

    public void Abort()
    {
        EndNode();
    }

    public virtual BTNode Get()
    {
        return this;
    }
}
