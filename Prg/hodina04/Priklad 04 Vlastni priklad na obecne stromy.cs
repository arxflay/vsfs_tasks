﻿using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace Priklad_04_Vlastni_priklad_na_obecne_stromy
{
    class TreeNode
    {
        public TreeNode[] children;
        public string value;
    }
    
    class Program
    {
        static TreeNode CreateNode(string value, int childCount = 2)
        {
            if (childCount <= 0)
                throw new Exception("childCount should be heighier than 0");

            TreeNode node = new TreeNode();
            node.children = new TreeNode[childCount];
            node.value = value;

            return node;
        }

        static void Extend(TreeNode node)
        {
            if (node == null)
                return;
            
            TreeNode[] newChildren = new TreeNode[node.children.Length * 2];

            for (int i = 0; i < node.children.Length; i++)
                newChildren[i] = node.children[i];

            node.children = newChildren;
        }

        static bool IsFull(TreeNode node)
        {
            for (int i = 0; i < node.children.Length; i++)
                if (node.children[i] == null)
                    return false;

            return true;
        }

        static TreeNode Insert(TreeNode root, string value)
        {
            TreeNode node = CreateNode(value);
            
            if (root == null)
                return node;

            if (IsFull(root))
            {
                int prevLength = root.children.Length;
                Extend(root);
                root.children[prevLength] = node;
            }
            else
            {
                for (int i = 0; i < root.children.Length; i++)
                {
                    if (root.children[i] == null)
                    {
                        root.children[i] = node;
                        break;
                    }
                }
            }

            return node;
                        
        }

        static TreeNode CreateProgTree(string filename)
        {
            if (!File.Exists(filename))
                return null;

            XDocument doc = XDocument.Load(filename);
            XElement rootXEl = doc.Element("lang");

            if (rootXEl == null)
                return null;

            string attr = rootXEl.Attribute("name").Value;

            if (attr == null)
                throw new Exception("element does not contain lang attribute"); 

            TreeNode root = Insert(null, attr);
            CreateProgTreeRec(root, rootXEl);

            return root;
        }

        static void CreateProgTreeRec(TreeNode node, XElement el)
        {
            if (el.Elements().Count() <= 0)
                return;

            XElement[] elChildren = el.Elements().ToArray();
            
            for (int i = 0; i < elChildren.Length; i++)
            {
                string attr = elChildren[i].Attribute("name").Value;

                if (attr == null)
                    throw new Exception("element does not contain lang attribute");

                TreeNode childNode = Insert(node, attr);
                CreateProgTreeRec(childNode, elChildren[i]);
            }

        }

        static int GetCount(TreeNode node)
        {
            int count = 0;
            for(int i = 0; i < node.children.Length; i++)
            {
                if (node.children[i] != null)
                    count++;
                else
                    return count;
            }

            return count;
        }

        static void WriteProgTree(TreeNode node)
        {
            if (node == null)
                return;

            WriteProgTreeRec(node, "", "");
        }

        static void WriteProgTreeRec(TreeNode node, string childSpaces, string depthSpaces = "")
        {
            int count = GetCount(node);

            Console.WriteLine("{0}{1}", childSpaces, node.value);

            for (int i = 0; i < count ; i++)
            {
                if (i == count - 1)
                    WriteProgTreeRec(node.children[i], depthSpaces + " =-", depthSpaces + "  ");
                else
                    WriteProgTreeRec(node.children[i], depthSpaces + " |-", depthSpaces + " | ");
            }
        }
        

        static void Main(string[] args)
        {
            TreeNode progTree = CreateProgTree("prog_tree.xml");
            WriteProgTree(progTree);
        }
    }
}
