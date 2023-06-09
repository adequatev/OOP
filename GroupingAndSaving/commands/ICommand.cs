using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab7_oop;

namespace Lab7_oop
{
    public abstract class ICommand
    {
        public abstract void execute(IShape shape);
        public abstract void unexecute();
        public abstract ICommand clone();
        public abstract IShape GetShape();
    }

    public class ResizeCommand : ICommand
    {
        private IShape? _shape;
        public int _x, _y;
        public ResizeCommand(int x, int y)
        {
            _x = x;
            _y = y;
            _shape = null;
        }

        public override ICommand clone()
        {
            return new ResizeCommand(_x, _y);
        }

        public override void execute(IShape shape)
        {
            Console.WriteLine("execute resize");
            _shape = shape;
            _shape?.resize(_x, _y);
        }

        public override void unexecute()
        {
            Console.WriteLine("unexec resize");
            _shape?.resize(-_x, -_y);
        }

        public override IShape GetShape() => _shape;
    }

    public class MoveCommand : ICommand
    {
        private IShape? _shape;
        private int _x, _y;
        public MoveCommand(int x, int y)
        {
            _x = x;
            _y = y;
            _shape = null;
        }

        public override ICommand clone()
        {
            return new MoveCommand(_x, _y);
        }

        public override void execute(IShape shape)
        {
            Console.WriteLine("execute move");
            _shape = shape;
            _shape?.move(_x, _y);
        }

        public override IShape GetShape() => _shape;


        public override void unexecute()
        {
            Console.WriteLine("unexec move");
            _shape?.move(-_x, -_y);
        }
    }

    public class RecolorCommand : ICommand
    {
        private IShape? _shape;
        private Color _oldColor;
        private Color _newColor;
        public RecolorCommand(Color c)
        {
            _newColor = c;
            _shape = null;
        }

        public override ICommand clone()
        {
            return new RecolorCommand(_newColor);
        }

        public override void execute(IShape shape)
        {
            Console.WriteLine("execute recolor");
            _shape = shape;
            _oldColor = _shape.colorMain;
            _shape?.recolor(_newColor);
        }

        public override IShape GetShape() => _shape;

        public override void unexecute()
        {
            Console.WriteLine("unexecute recolor");
            _shape?.recolor(_oldColor);
        }
    }

    public class GroupCommand : ICommand
    {
        public List<IShape> shapes;
        public IShape added;

        public GroupCommand(List<IShape> shapes, IShape added = null)
        {
            this.shapes = shapes;
            this.added = added;
        }

        public override ICommand clone()
        {
            return new GroupCommand(shapes);
        }

        public override void execute(IShape shape)
        {
            if (shape == null) return;
            added = shape;
            ((shapeGroup)added).shapes.Clear();
            foreach (var s in shapes)
                if (s.IsSelected) ((shapeGroup)added).AddShape(s);
            List<IShape> b = new();

            foreach (var s in shapes)
                if (s.IsSelected) b.Add(s);
            foreach (var s in b)
                shapes.Remove(s);
            if(((shapeGroup)added).shapes.Count > 0)
            {
                shapes.Add(added);
            }
            
        }

        public override IShape GetShape() => added;

        public override void unexecute()
        {
            List<IShape> grElems = new();
            ((shapeGroup)added).Ungroup(grElems);
            shapes.Remove(added);
            foreach (var s in grElems)
                shapes.Add(s);
                
        }
    }

    public class UnGroupCommand : ICommand
    {
        public List<IShape> shapes;
        public IShape added;
        public UnGroupCommand(List<IShape> _selection)
        {
            shapes = _selection;
        }

        public override ICommand clone()
        {
            return new UnGroupCommand(shapes);
        }

        public override void execute(IShape shape)
        {
            added = shape;
            ICommand c = new GroupCommand(shapes, added);
            c.unexecute();
        }

        public override IShape GetShape() => added;           
        

        public override void unexecute()
        {
            ICommand c = new GroupCommand(shapes);
            c.execute(added);
        }
    }

    public class CreateShapeCommand : ICommand
    {
        private IShape? _shape;
        private List<IShape> _shapes;
        public int _x, _y, _type, _index;
        public CreateShapeCommand(int x, int y, int type, List<IShape> shapes)
        {
            _x = x;
            _y = y;
            _type = type;
            _shapes = shapes;
        }
        public override ICommand clone()
        {
            return new CreateShapeCommand(_x, _y, _type, _shapes);
        }

        public override void execute(IShape shape)
        {
            Console.WriteLine("createShapeCommand execute");
            _shape = shapeCreator.createShapeMethod(_x, _y, _type);
            _shape.IsSelected = true;
            _shapes.Add(_shape);

        }

        public override IShape GetShape() => _shape;
        public override void unexecute()
        {
            Console.WriteLine("createShapeCommand unexecute");
            _shapes.Remove(_shape);
            //_shapes.RemoveAt(_index);
            
        }
    }
    public class DeleteShapeCommand : ICommand
    {
        private IShape? _shape;
        private List<IShape> _shapes, saveBuffer, deleteBuffer;
        public DeleteShapeCommand(List<IShape> shapes)
        {
            _shapes = shapes;
        }
        public override ICommand clone()
        {
            return new DeleteShapeCommand(_shapes);
        }

        public override void execute(IShape shape)
        {
            Console.WriteLine("execute delete shape cmd ");
            saveBuffer = new();
            deleteBuffer = new();
            foreach (IShape c in _shapes)
            {
                //saving all non-selected
                if (!c.IsSelected) saveBuffer.Add(c);
                else deleteBuffer.Add(c);
            }
            _shapes.Clear();
            
            foreach (IShape c in saveBuffer)
                _shapes.Add(c);
            Console.WriteLine("deleted");
        }

        public override IShape GetShape() => _shape;
        public override void unexecute()
        {
            Console.WriteLine("unexecute delete shape cmd ");
            foreach (IShape s in deleteBuffer)
                _shapes.Add(s);
            

        }
    }


}
