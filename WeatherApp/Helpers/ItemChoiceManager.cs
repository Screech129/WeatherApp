using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Java.Lang;
namespace WeatherApp
{
    public class ItemChoiceManager : AbsListView
    {
        private string logTag = "Item Choice Manager";
        private string selectedItemsKey = "SIK";
        private int choiceMode;
        private Context context;
        private readonly RecyclerView.Adapter adapter;
        private CustomDataObserver dataObserver;
        private ItemChoiceManager (Context context) : base(context)
        {
            this.context = context;
        }



        public ItemChoiceManager (Context context, RecyclerView.Adapter adapter) : base(context)
        {
            this.adapter = adapter;
            dataObserver = new CustomDataObserver(adapter,this);
        }

        public class CustomDataObserver : RecyclerView.AdapterDataObserver
        {
            private readonly RecyclerView.Adapter adapter;
            private readonly ItemChoiceManager manager;
            public CustomDataObserver (RecyclerView.Adapter adapter, ItemChoiceManager manager)
            {
                this.adapter = adapter;
                this.manager = manager;
            }

            public override void OnChanged ()
            {
                base.OnChanged();
                if (adapter != null && adapter.HasStableIds)
                    manager.ConfirmCheckedPositionsById(adapter.ItemCount);
            }
        }

        

        /**
         * How many positions in either direction we will search to try to
         * find a checked item with a stable ID that moved position across
         * a data set change. If the item isn't found it will be unselected.
         */
        private const int CheckPositionSearchDistance = 20;

        /**
         * Running state of which positions are currently checked
         */
        private SparseBooleanArray checkStates = new SparseBooleanArray();

        /**
         * Running state of which IDs are currently checked.
         * If there is a value for a given key, the checked state for that ID is true
         * and the value holds the last known position in the adapter for that id.
         */
        private readonly LongSparseArray checkedIdStates = new LongSparseArray();

        public void OnClick (RecyclerView.ViewHolder vh)
        {

            if (choiceMode == (int)ChoiceMode.None)
                return;

            var checkedItemCount = checkStates.Size();
            var position = vh.AdapterPosition;

            if (position == RecyclerView.NoPosition)
            {
                Log.Debug(logTag, "Unable to Set Item State");
                return;
            }

            switch (choiceMode)
            {
                case (int)ChoiceMode.None:
                    break;
                case (int)ChoiceMode.Single:
                    {
                        var isChecked = checkStates.Get(position, false);
                        if (!isChecked)
                        {
                            for (var i = 0; i < checkedItemCount; i++)
                            {
                                adapter.NotifyItemChanged(checkStates.KeyAt(i));
                            }
                            checkStates.Clear();
                            checkStates.Put(position, true);
                            checkedIdStates.Clear();
                            checkedIdStates.Put(adapter.GetItemId(position), position);
                        }
                        // We directly call OnBindViewHolder here because notifying that an item has
                        // changed on an item that has the focus causes it to lose focus, which makes
                        // keyboard navigation a bit annoying
                        adapter.OnBindViewHolder(vh, position);
                        break;
                    }
                case (int)ChoiceMode.Multiple:
                    {
                        var isChecked = checkStates.Get(position, false);
                        checkStates.Put(position, !isChecked);
                        // We directly call OnBindViewHolder here because notifying that an item has
                        // changed on an item that has the focus causes it to lose focus, which makes
                        // keyboard navigation a bit annoying
                        adapter.OnBindViewHolder(vh, position);
                        break;
                    }
                case (int)ChoiceMode.MultipleModal:
                    {
                        throw new RuntimeException("Multiple Modal not implemented in ItemChoiceManager.");
                    }
            }
        }

        /**
         * Defines the choice behavior for the RecyclerView. By default, RecyclerViewChoiceMode does
         * not have any choice behavior (AbsListView.CHOICE_MODE_NONE). By setting the choiceMode to
         * AbsListView.CHOICE_MODE_SINGLE, the RecyclerView allows up to one item to  be in a
         * chosen state.
         *
         * @param choiceMode One of AbsListView.CHOICE_MODE_NONE, AbsListView.CHOICE_MODE_SINGLE
         */
        public void SetChoiceMode (int choiceMode)
        {
            if (this.choiceMode != choiceMode)
            {
                this.choiceMode = choiceMode;
                ClearSelections();
            }
        }

        /**
         * Returns the checked state of the specified position. The result is only
         * valid if the choice mode has been set to AbsListView.CHOICE_MODE_SINGLE,
         * but the code does not check this.
         *
         * @param position The item whose checked state to return
         * @return The item's checked state
         * @see #setChoiceMode(int)
         */
        public override bool IsItemChecked (int position)
        {
            return checkStates.Get(position);
        }

        private void ClearSelections ()
        {
            checkStates.Clear();
            checkedIdStates.Clear();
        }

        private void ConfirmCheckedPositionsById (int oldItemCount)
        {
            // Clear out the positional check states, we'll rebuild it below from IDs.
            checkStates.Clear();

            for (var checkedIndex = 0; checkedIndex < checkedIdStates.Size(); checkedIndex++)
            {
                var id = checkedIdStates.KeyAt(checkedIndex);
                var lastPos = (int)checkedIdStates.ValueAt(checkedIndex);

                var lastPosId = adapter.GetItemId(lastPos);
                if (id != lastPosId)
                {
                    // Look around to see if the ID is nearby. If not, uncheck it.
                    var start = Math.Max(0, lastPos - CheckPositionSearchDistance);
                    var end = Math.Min(lastPos + CheckPositionSearchDistance, oldItemCount);
                    var found = false;
                    for (var searchPos = start; searchPos < end; searchPos++)
                    {
                        var searchId = adapter.GetItemId(searchPos);
                        if (id == searchId)
                        {
                            found = true;
                            checkStates.Put(searchPos, true);
                            checkedIdStates.SetValueAt(checkedIndex, searchPos);
                            break;
                        }
                    }

                    if (!found)
                    {
                        checkedIdStates.Delete(id);
                        checkedIndex--;
                    }
                }
                else
                {
                    checkStates.Put(lastPos, true);
                }
            }
        }

        public void OnBindViewHolder (RecyclerView.ViewHolder vh, int position)
        {
            var isChecked = IsItemChecked(position);
            if (vh.ItemView.GetType() == typeof(ICheckable))
            {
                ((ICheckable)vh.ItemView).Checked = isChecked;
            }
            ViewCompat.SetActivated(vh.ItemView, isChecked);
        }

        public void OnRestoreInstanceState (Bundle savedInstanceState)
        {
            var states = savedInstanceState.GetByteArray(selectedItemsKey);
            if (null != states)
            {
                var inParcel = Parcel.Obtain();
                inParcel.Unmarshall(states, 0, states.Length);
                inParcel.SetDataPosition(0);
                checkStates = inParcel.ReadSparseBooleanArray();
                var numStates = inParcel.ReadInt();
                checkedIdStates.Clear();
                for (var i = 0; i < numStates; i++)
                {
                    var key = inParcel.ReadLong();
                    var value = inParcel.ReadInt();
                    checkedIdStates.Put(key, value);
                }
            }
        }

        public void OnSaveInstanceState (Bundle outState)
        {
            var outParcel = Parcel.Obtain();
            outParcel.WriteSparseBooleanArray(checkStates);
            var numStates = checkedIdStates.Size();
            outParcel.WriteInt(numStates);
            for (var i = 0; i < numStates; i++)
            {
                outParcel.WriteLong(checkedIdStates.KeyAt(i));
                outParcel.WriteInt((int)checkedIdStates.ValueAt(i));
            }
            var states = outParcel.Marshall();
            outState.PutByteArray(selectedItemsKey, states);
            outParcel.Recycle();
        }

        public int GetSelectedItemPosition ()
        {
            if (checkStates.Size() == 0)
            {
                return RecyclerView.NoPosition;
            }
            else
            {
                return checkStates.KeyAt(0);
            }
        }

        public override void SetSelection (int position)
        {
            throw new System.NotImplementedException();
        }

        public override IListAdapter Adapter { get; set; }
    }


}