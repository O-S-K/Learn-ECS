using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IMessage
{
}

public class MessageBus
{
    private static Dictionary<Type, List<Action<IMessage>>> subscribers = new Dictionary<Type, List<Action<IMessage>>>();

    public static void Subscribe<T>(Action<T> action) where T : IMessage
    {
        if (subscribers.ContainsKey(typeof(T)))
        {
            subscribers.Add(typeof(T), new List<Action<IMessage>>());
        }
        subscribers[typeof(T)].Add(mess => action((T)mess));
    }


    public static void Unsubscribe<T>(Action<T> action) where T : IMessage
    {
        Type messType = typeof(T);
        if (subscribers.ContainsKey(messType))
        {
            List<Action<IMessage>> actionList = subscribers[messType];
            List<Action<IMessage>> actionToRemove = new List<Action<IMessage>>();

            foreach (var sub in actionList)
            {
                if(sub.Target == action.Target)
                {
                    actionToRemove.Add(sub);
                }
            }

            foreach (var actionRemove in actionToRemove)
            {
                actionList.Remove(actionRemove);
            }
        }
    }


    public static void Publish(IMessage message)
    {
        Type type = message.GetType();

        if (subscribers.ContainsKey(type))
        {
            foreach (var subscriber in subscribers[type])
            {
                subscriber(message);
            }
        }
    }
}

