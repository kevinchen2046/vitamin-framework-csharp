using System;
using System.Reflection;

namespace vitamin
{
    public abstract class ModelBase : EventEmitter
    {
        public ModelBase(){}
        public virtual void initialize() { }
        public virtual void reset() { }
    }

    public abstract class ViewBase : EventEmitter
    {
        public ViewBase(){}

        public virtual void enter(){
            //Vitamin.addCommandEnd(this);
        }

        public virtual void exit(){
            //Vitamin.removeCommandEnd(this);
        }

        protected void execCommand(string cmdRoute, params object[] args){
            Vitamin.execCommand(cmdRoute,args);
        }
    }

    public abstract class CommandBase : EventEmitter
    {
        public Net net;
        public CommandBase(){}
        public virtual void exec(params object[] args){}
    }
}