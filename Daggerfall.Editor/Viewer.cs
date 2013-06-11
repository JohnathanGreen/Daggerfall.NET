using System;
using Gtk;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using Microsoft.Xna.Framework;

namespace Daggerfall.Editor
{
    public partial class Viewer : Gtk.Window
    {
        public Viewer()
            : base(Gtk.WindowType.Toplevel)
        {
            this.Build();

            state = new Daggerfall.State(null, @"D:\Games\Daggerfall\ARENA2\");
            var models = state.Models;
            var regions = state.Regions;

            blockChunkList.AppendColumn("Model", new CellRendererText(), "text", 0);
            blockChunkList.AppendColumn("Position", new CellRendererText(), "text", 1);
            blockChunkList.AppendColumn("Angle", new CellRendererText(), "text", 2);
            blockChunkList.AppendColumn("Building", new CellRendererText(), "text", 3);
            blockChunkList.AppendColumn("Unknown", new CellRendererText(), "text", 4);
            blockChunkList.NodeStore = new NodeStore(typeof(ExteriorChunkNodeState));

            blockList.AppendColumn("Chunks", new CellRendererText() { }, "text", 0);
            blockList.AppendColumn("U1", new CellRendererText() { }, "text", 1);
            blockList.NodeStore = new NodeStore(typeof(BlockNodeState));
            blockList.VisibilityNotifyEvent += OnBlocksPaneShown;

            blockList.Selection.Changed += OnBlockListSelectionChanged;

            glwidget.RenderFrame += OnGLRenderFrame;
            game = new GtkGame(glwidget);
        }

        GtkGame game;

        void OnGLRenderFrame(object sender, EventArgs e)
        {
            GL.ClearColor(1, 0, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        Daggerfall.State state;

        Block CurrentBlock
        {
            get
            {
                var rows = blockList.Selection.GetSelectedRows();
                if (rows == null || rows.Length == 0) return null;
                TreePath path = rows[0];
                return BlockStore[path.Indices[0]];
            }
        }

        void OnBlockListSelectionChanged(object sender, EventArgs e)
        {
            var block = CurrentBlock;

            blockChunkList.NodeStore.Clear();
            if (block == null) return;
            GC.Collect();

            ExteriorBlock exterior = block as ExteriorBlock;
            if (exterior != null)
            {
                for (int index = 0; index < exterior.ChunkCount; index++)
                {
                    ExteriorChunk chunk = exterior.Chunks[index];
                    blockChunkList.NodeStore.AddNode(new ExteriorChunkNodeState(chunk));
                }
            }
        }

        protected void OnBlockListRowActivated(object o, RowActivatedArgs args) { }

        protected void OnBlockListExposed(object o, ExposeEventArgs args)
        {
            OnBlocksPaneShown(null, null);
        }

        bool blockListSetup;
        List<Block> BlockStore = new List<Block>();

        protected void OnBlocksPaneShown(object sender, EventArgs e)
        {
            if (blockListSetup) return;
            blockListSetup = true;

            BlockStore.Clear();
            blockList.NodeStore.Clear();
            var blocks = state.Blocks.Records;
            foreach (Block item in blocks)
            {
                if (item is ExteriorBlock)
                {
                    BlockStore.Add(item);
                    blockList.NodeStore.AddNode(new BlockNodeState((ExteriorBlock)item));
                }
            }

        }
    }

    [Gtk.TreeNode(ListOnly = true)]
    class BlockNodeState : Gtk.TreeNode
    {
        public BlockNodeState(ExteriorBlock block)
        {
            this.block = block;
        }

        readonly ExteriorBlock block;

        [Gtk.TreeNodeValue(Column = 0)]
        public string ChunkCount { get { return block.ChunkCount.ToString(); } }

        [Gtk.TreeNodeValue(Column = 1)]
        public string U1 { get { return block.U1.ToHexString(); } }
    }

    [Gtk.TreeNode(ListOnly = true)]
    class ExteriorChunkNodeState : Gtk.TreeNode
    {
        public ExteriorChunkNodeState(ExteriorChunk chunk)
        {
            this.chunk = chunk;
        }

        readonly ExteriorChunk chunk;

        [Gtk.TreeNodeValue(Column = 0)]
        public string Model { get { return chunk.ModelFilename; } }

        [Gtk.TreeNodeValue(Column = 1)]
        public string Position { get { return string.Format("{0}×{1}", chunk.X, chunk.Z); } }

        [Gtk.TreeNodeValue(Column = 2)]
        public string Angle { get { return chunk.Angle.InDegrees + "°"; } }

        [Gtk.TreeNodeValue(Column = 3)]
        public string Building { get { return chunk.Building.ToString(); } }

        [Gtk.TreeNodeValue(Column = 4)]
        public string Unknown { get { return chunk.Unknowns.ToString(); } }
    }
}

