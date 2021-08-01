using System;
using System.Collections.Generic;
using System.IO;

namespace ACA.Config.CCFG // TODO: Move to ACA (new project: ACA.Config.CCFG.Objects)
{
    /// <summary>
    /// Types of comments.
    /// </summary>
    public enum CommentType
    {
        /// <summary>
        /// Pre-category comment (before the category start but after the category start keytag, categories-only)
        /// </summary>
        PreCategory = 0,

        /// <summary>
        /// Pre-id comment (before the id of the item or category)
        /// </summary>
        PreID = 1,

        /// <summary>
        /// Post-value comment (after the value or category end keytag)
        /// </summary>
        PostValue = 2,
    }

    /// <summary>
    /// CCFG Category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Category"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public Category(string name)
        {
            this.name = name;
            items = new List<Item>();
            subcategories = new List<Category>();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Category"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="items">Items.</param>
        public Category(string name, List<Item> items)
        {
            this.name = name;
            this.items = items;
            subcategories = new List<Category>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Category"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="items">Items.</param>
        /// <param name="categories">Categories.</param>
        public Category(string name, List<Item> items, List<Category> categories)
        {
            this.name = name;
            this.items = items;
            subcategories = categories;
        }

        internal string name;
        internal List<Item> items;
        internal List<Category> subcategories;
        internal List<string> PreKeyComments = new List<string>();
        internal List<string> PreCategoryComments = new List<string>();
        internal List<string> PostValueComments = new List<string>();

        /// <summary>
        /// Get comments by type.
        /// </summary>
        /// <returns>Comments by type.</returns>
        /// <param name="type">Comment type.</param>
        public List<string> GetComments(CommentType type)
        {
            if (type == CommentType.PreID) return new List<string>(PreKeyComments);
            else if (type == CommentType.PreCategory) return new List<string>(PreCategoryComments);
            else if (type == CommentType.PostValue) return new List<string>(PostValueComments);
            else throw new InvalidOperationException("Invalid comment type");
        }
        /// <summary>
        /// Add comment.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        /// <param name="ignoreexistence">If set to <c>true</c>, allow adding even if there is already a comment with the same text.</param>
        public void AddComment(string comment, CommentType type, bool ignoreexistence = false)
        {
            if (type == CommentType.PreCategory)
            {
                if (PreCategoryComments.Contains(comment) && !ignoreexistence) throw new ArgumentException("Comment with same text already exists");
                PreCategoryComments.Add(comment);
            }
            else if (type == CommentType.PreID)
            {
                if (PreKeyComments.Contains(comment) && !ignoreexistence) throw new ArgumentException("Comment with same text already exists");
                PreKeyComments.Add(comment);
            }
            else if (type == CommentType.PostValue)
            {
                if (PostValueComments.Contains(comment) && !ignoreexistence) throw new ArgumentException("Comment with same text already exists");
                PostValueComments.Add(comment);
            }
        }
        /// <summary>
        /// Remove comment.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        public void RemoveComment(string comment, CommentType type)
        {
            if (type == CommentType.PreCategory)
            {
                if (!PreCategoryComments.Contains(comment)) throw new ArgumentException("Comment not found");
                PreCategoryComments.Remove(comment);
            }
            else if (type == CommentType.PreID)
            {
                if (!PreKeyComments.Contains(comment)) throw new ArgumentException("Comment not found");
                PreKeyComments.Remove(comment);
            }
            else if (type == CommentType.PostValue)
            {
                if (!PostValueComments.Contains(comment)) throw new ArgumentException("Comment not found");
                PostValueComments.Remove(comment);
            }
        }
        /// <summary>
        /// Check if a comment exists.
        /// </summary>
        /// <returns><c>true</c>, if comment was found, <c>false</c> otherwise.</returns>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        public bool ContainsComment(string comment, CommentType type)
        {
            if (type == CommentType.PreID) return PreKeyComments.Contains(comment);
            else if (type == CommentType.PreCategory) return PreCategoryComments.Contains(comment);
            else if (type == CommentType.PostValue) return PostValueComments.Contains(comment);
            else throw new InvalidOperationException("Invalid comment type");
        }

        /// <summary>
        /// Get the category name
        /// </summary>
        /// <returns>The name of the co.</returns>
        public string GetName() => name;
        /// <summary>
        /// Get all sub-categories.
        /// </summary>
        /// <returns>The categories inside this category.</returns>
        public List<Category> GetCategories() => subcategories;
        /// <summary>
        /// Get all items inside the category.
        /// </summary>
        /// <returns>The items.</returns>
        public List<Item> GetItems() => items;

        /// <summary>
        /// Get item by ID.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="id">Item identifier.</param>
        public Item GetItem(string id) => items.Find(t => t.id == id);
        /// <summary>
        /// Get category by name
        /// </summary>
        /// <returns>The category.</returns>
        /// <param name="name">Category name.</param>
        public Category GetCategory(string name) => subcategories.Find(t => t.name == name);
        /// <summary>
        /// Get item by the ID of an item object.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="i">Item object.</param>
        public Item Get(Item i) => GetOrAdd(i);
        /// <summary>
        /// Get category by the name of a category object.
        /// </summary>
        /// <returns>The category.</returns>
        /// <param name="i">Category object.</param>
        public Category Get(Category i) => GetCategory(i.name);

        /// <summary>
        /// Check item's existence.
        /// </summary>
        /// <returns><c>true</c>, if item was found, <c>false</c> otherwise.</returns>
        /// <param name="id">Item identifier.</param>
        public bool ContainsItem(string id) => items.Find(t => t.id == id) != null;
        /// <summary>
        /// Check category's existence.
        /// </summary>
        /// <returns><c>true</c>, if category was found, <c>false</c> otherwise.</returns>
        /// <param name="name">Category name.</param>
        public bool ContainsCategory(string name) => subcategories.Find(t => t.name == name) != null;
        /// <summary>
        /// Check if the category contains the specified item object (checks by id).
        /// </summary>
        /// <returns>The item obect.</returns>
        /// <param name="i">Input item object.</param>
        public bool Contains(Item i) => ContainsItem(i.id);
        /// <summary>
        /// Check if the category contains the specified sub-category object (checks by name).
        /// </summary>
        /// <returns>The category obect.</returns>
        /// <param name="i">Input category object.</param>
        public bool Contains(Category i) => ContainsCategory(i.name);

        /// <summary>
        /// Gets a category, creates a new category if it could not found.
        /// </summary>
        /// <returns>Category object.</returns>
        /// <param name="name">Category name.</param>
        /// <param name="defaultcat">Default category object. (used if the category was not found, null creates a new one)</param>
        public Category GetOrAddCategory(string name, Category defaultcat = null)
        {
            if (defaultcat == null) defaultcat = new Category(name);
            defaultcat.name = name;
            return (Contains(defaultcat) ? Get(defaultcat) : AddCategory(name, defaultcat));
        }
        //TODO: More XML Documentation
        public Category GetOrAddCategory(Category category) => GetOrAddCategory(category.name, category);

        public Category AddCategory(string name, Category cat = null)
        {
            if (cat == null) cat = new Category(name);
            cat.name = name;
            if (Contains(cat)) throw new ArgumentException("Category already exists.");
            subcategories.Add(cat);
            return cat;
        }
        public Category SetCategory(string name, Category cat = null)
        {
            if (cat == null) cat = new Category(name);
            cat.name = name;
            if (Contains(cat)) subcategories[subcategories.IndexOf(subcategories.Find(t => t.name == name))] = cat;
            else subcategories.Add(cat);
            return cat;
        }

        public Item GetOrAddItem(string id, string defaultvalue = "")
        {
            Item itm = new Item(id, defaultvalue);
            return (Contains(itm) ? Get(itm) : SetItem(itm.id, itm.value));
        }
        public Item GetOrAddItem(string id, Item defaultitm = null)
        {
            if (defaultitm == null) defaultitm = new Item(id);
            defaultitm.id = id;
            return (Contains(defaultitm) ? Get(defaultitm) : SetItem(defaultitm.id, defaultitm.value));
        }
        public Item GetOrAddItem(Item itm) => GetOrAddItem(itm.id, itm);

        public Item AddItem(string id, string value = "")
        {
            Item itm = new Item(id, value);
            if (Contains(itm)) throw new ArgumentException("Item already exists.");
            items.Add(itm);
            return itm;
        }
        public Item SetItem(string id, string value = "")
        {
            Item itm = new Item(id, value);
            if (Contains(itm)) items[items.IndexOf(items.Find(t => t.id == id))] = itm;
            else items.Add(itm);
            return itm;
        }

        public Item Set(Item item) => SetItem(item.id, item.value);
        public Item Add(Item item) => AddItem(item.id, item.value);
        public Item Set(string id, string value) => SetItem(id, value);
        public Item Add(string id, string value) => AddItem(id, value);

        public Category Set(Category cat) => SetCategory(cat.name, cat);
        public Category Set(string name, List<Item> items) => SetCategory(name, new Category(name, items));
        public Category Set(string name, List<Item> items, List<Category> categories) => SetCategory(name, new Category(name, items, categories));
        public Category Add(Category cat) => AddCategory(cat.name, cat);
        public Category Add(string name, List<Item> items) => AddCategory(name, new Category(name, items));
        public Category Add(string name, List<Item> items, List<Category> categories) => AddCategory(name, new Category(name, items, categories));

        public Item GetOrAdd(Item defaultitm) => GetOrAddItem(defaultitm);
        public Item GetOrAdd(string id, string defaultvalue) => GetOrAddItem(id, defaultvalue);

        public Category GetOrAdd(Category defaultcat) => GetOrAddCategory(defaultcat);
        public Category GetOrAdd(string name, List<Item> items) => GetOrAddCategory(name, new Category(name, items));
        public Category GetOrAdd(string name, List<Item> items, List<Category> categories) => GetOrAddCategory(name, new Category(name, items, categories));

        public void RemoveItem(string id)
        {
            if (!ContainsItem(id)) throw new ArgumentException($"Could not find item '{id}'");
            items.Remove(items.Find(t => t.id == id));
        }
        public void RemoveCategory(string name)
        {
            if (!ContainsCategory(na: me)) throw new ArgumentException($"Could not find category '{name}'");
            subcategories.Remove(subcategories.Find(t => t.name == name));
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> in the CCFG config format, created from current config values."/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> in the CCFG config format, created from current config values.</returns>
        public override string ToString()
        {
            return ToString(0);
        }

        public String ToString(int indent) {
            StringWriter writer = new StringWriter();
            for (int i = 0; i < indent; i++)
                writer.Write("    ");

            writer.WriteLine(name + "> {");
            bool hasComments = false;
            foreach (String comment in GetComments(CommentType.PreCategory))
            {
                for (int i = 0; i < indent + 1; i++)
                    writer.Write("    ");
                writer.WriteLine("# " + comment);
                hasComments = true; 
            }
            if (hasComments)
                writer.WriteLine();
            bool firstItem = true;
            foreach (Item itm in items)
            {
                bool firstItm = true;
                foreach (String comment in itm.GetComments(CommentType.PreID))
                {
                    if (firstItm && (!firstItem || !hasComments))
                    {
                        writer.WriteLine();
                    }

                    firstItm = false;
                    for (int i = 0; i < indent + 1; i++)
                        writer.Write("    ");
                    writer.WriteLine("# " + comment);
                }
                for (int i = 0; i < indent + 1; i++)
                    writer.Write("    ");
               
                String value = "";
                bool firstV = true;
                foreach (String line in itm.value.Split('\n'))
                {
                    if (!firstV)
                        for (int i = 0; i < indent + 1; i++)
                           value += "    ";
                    value += line;
                    firstV = false;
                }
                writer.Write(itm.id + "> '" + value + "'");
                firstV = true;
                foreach (String comment in itm.GetComments(CommentType.PostValue))
                {
                    if (!firstV)
                    {
                        for (int i = 0; i < indent + 1; i++)
                            value += "    ";
                    }
                    else
                        writer.Write(" ");
                    writer.WriteLine("# " + comment);
                    firstV = false;
                }
                if (!firstV)
                {
                    writer.WriteLine();
                    writer.WriteLine();
                }
                else
                    writer.WriteLine();
                firstItem = false;
            }

            foreach (Category cat in subcategories)
            {
                foreach (String comment in cat.GetComments(CommentType.PreID))
                {
                    for (int i = 0; i < indent + 1; i++)
                        writer.Write("    ");
                    writer.WriteLine("# " + comment);
                }
                writer.WriteLine(cat.ToString(indent + 1));
            }
            writer.Write("}");

            bool first = true;
            foreach (String comment in GetComments(CommentType.PostValue))
            {
                if (!first)
                {
                    for (int i = 0; i < indent + 1; i++)
                        writer.Write("    ");
                }
                writer.WriteLine("# " + comment);
                first = false;
            }
            if (first)
                writer.WriteLine();
            return writer.ToString();
        }

        public void Remove(Item i) => RemoveItem(i.id);
        public void Remove(Category c) => RemoveCategory(c.name);
    }

    /// <summary>
    /// CCFG Item
    /// </summary>
    public class Item
    {
        string _id;
        public string id
        {
            get { return GetID(); }
            internal set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets the item identifier.
        /// </summary>
        /// <returns>The item identifier.</returns>
        public string GetID() => _id;

        /// <summary>
        /// The value.
        /// </summary>
        public string value;
        internal bool isComment;

        /// <summary>
        /// Check of the item is a comment only.
        /// </summary>
        /// <returns><c>true</c>, if the item is a comment, <c>false</c> otherwise.</returns>
        public bool GetIsComment() => isComment;

        internal List<string> PreKeyComments = new List<string>();
        internal List<string> PostValueComments = new List<string>();

        /// <summary>
        /// Get comments by type.
        /// </summary>
        /// <returns>Comments by type.</returns>
        /// <param name="type">Comment type.</param>
        public List<string> GetComments(CommentType type)
        {
            if (type == CommentType.PreID) return new List<string>(PreKeyComments);
            else if (type == CommentType.PostValue) return new List<string>(PostValueComments);
            else throw new InvalidOperationException("Invalid comment type");
        }
        /// <summary>
        /// Add comment.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        /// <param name="ignoreexistence">If set to <c>true</c>, allow adding even if there is already a comment with the same text.</param>
        public void AddComment(string comment, CommentType type, bool ignoreexistence = false)
        {
            if (type == CommentType.PreID)
            {
                if (PreKeyComments.Contains(comment) && !ignoreexistence) throw new ArgumentException("Comment with same text already exists");
                PreKeyComments.Add(comment);
            }
            else if (type == CommentType.PostValue)
            {
                if (PostValueComments.Contains(comment) && !ignoreexistence) throw new ArgumentException("Comment with same text already exists");
                PostValueComments.Add(comment);
            }
            else throw new InvalidOperationException("Invalid comment type");
        }
        /// <summary>
        /// Remove comment.
        /// </summary>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        public void RemoveComment(string comment, CommentType type)
        {
            if (type == CommentType.PreID)
            {
                if (!PreKeyComments.Contains(comment)) throw new ArgumentException("Comment not found");
                PreKeyComments.Remove(comment);
            }
            else if (type == CommentType.PostValue)
            {
                if (!PostValueComments.Contains(comment)) throw new ArgumentException("Comment not found");
                PostValueComments.Remove(comment);
            }
            else throw new InvalidOperationException("Invalid comment type");
        }
        /// <summary>
        /// Check if a comment exists.
        /// </summary>
        /// <returns><c>true</c>, if comment was found, <c>false</c> otherwise.</returns>
        /// <param name="comment">Comment.</param>
        /// <param name="type">Comment type.</param>
        public bool ContainsComment(string comment, CommentType type)
        {
            if (type == CommentType.PreID) return PreKeyComments.Contains(comment);
            else if (type == CommentType.PostValue) return PostValueComments.Contains(comment);
            else throw new InvalidOperationException("Invalid comment type");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Item"/> class.
        /// </summary>
        /// <param name="id">Item identifier.</param>
        public Item(string id) { this.id = id; value = ""; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Item"/> class.
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <param name="isComment">If set to <c>true</c>, the item is comment item.</param>
        public Item(string id, bool isComment) { this.id = id; this.isComment = isComment; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ACA.Config.CCFG.Item"/> class.
        /// </summary>
        /// <param name="id">Item identifier.</param>
        /// <param name="value">Item value.</param>
        public Item(string id, string value) { this.id = id; this.value = value; }
    }
}
