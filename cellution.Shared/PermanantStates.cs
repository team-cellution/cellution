using System;
using System.Collections.Generic;
using System.Text;

namespace cellution
{
    public class PermanantStates<T>
    {
        private Dictionary<string, T> states;
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (states.ContainsKey(value))
                {
                    name = value;
                    CurrentState = states[Name];
                }
            }
        }
        public T CurrentState { get; private set; }

        public PermanantStates()
        {
            states = new Dictionary<string, T>();
            name = null;
            CurrentState = default(T);
        }

        public void AddState(string name, T state)
        {
            if (!states.ContainsKey(name))
            {
                states.Add(name, state);
            }

            if (Name == null)
            {
                Name = name;
            }
        }

        public T GetState(string name)
        {
            if (states.ContainsKey(name))
            {
                return states[name];
            }
            else
            {
                throw new Exception("That state doesn't exist");
            }
        }

        public void RemoveState(string name)
        {
            if (states.ContainsKey(name))
            {
                if (Name == name)
                {
                    Name = null;
                    CurrentState = default(T);
                }
                states.Remove(name);
            }
        }
    }
}
