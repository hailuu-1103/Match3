using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusItem : Item
{
    public enum eBonusType
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    public eBonusType eItemType;

    public void SetType(eBonusType type)
    {
        this.eItemType = type;
    }

    public override string Type => this.eItemType.ToString();

    internal override bool IsSameType(Item other)
    {
        BonusItem it = other as BonusItem;

        return it != null && it.eItemType == this.eItemType;
    }

    internal override void ExplodeView()
    {
        ActivateBonus();

        base.ExplodeView();
    }

    private void ActivateBonus()
    {
        switch (this.eItemType)
        {
            case eBonusType.HORIZONTAL:
                ExplodeHorizontalLine();
                break;
            case eBonusType.VERTICAL:
                ExplodeVerticalLine();
                break;
            case eBonusType.ALL:
                ExplodeBomb();
                break;

        }
    }

    private void ExplodeBomb()
    {
        List<Cell> list = new List<Cell>();
        if (Cell.NeighbourBottom) list.Add(Cell.NeighbourBottom);
        if (Cell.NeighbourUp) list.Add(Cell.NeighbourUp);
        if (Cell.NeighbourLeft)
        {
            list.Add(Cell.NeighbourLeft);
            if (Cell.NeighbourLeft.NeighbourUp)
            {
                list.Add(Cell.NeighbourLeft.NeighbourUp);
            }
            if (Cell.NeighbourLeft.NeighbourBottom)
            {
                list.Add(Cell.NeighbourLeft.NeighbourBottom);
            }
        }
        if (Cell.NeighbourRight)
        {
            list.Add(Cell.NeighbourRight);
            if (Cell.NeighbourRight.NeighbourUp)
            {
                list.Add(Cell.NeighbourRight.NeighbourUp);
            }
            if (Cell.NeighbourRight.NeighbourBottom)
            {
                list.Add(Cell.NeighbourRight.NeighbourBottom);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }
    }

    private void ExplodeVerticalLine()
    {
        List<Cell> list = new List<Cell>();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourUp;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourBottom;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }


        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }
    }

    private void ExplodeHorizontalLine()
    {
        List<Cell> list = new List<Cell>();

        Cell newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourRight;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }

        newcell = Cell;
        while (true)
        {
            Cell next = newcell.NeighbourLeft;
            if (next == null) break;

            list.Add(next);
            newcell = next;
        }


        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }

    }
}
