<<<<<<< HEAD
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static MathAPI.Helpers.MatrixHelpers;
using static MathAPI.Helpers.LinearSystemHelpers;

namespace MathAPI.MathTypes
{
    public readonly struct LinearSystem
    {
        public LinearSystem (Matrix<Fraction> equations)
        {
            // Adjusted as the equations are reduced and solved.
            IsHomogenous = false;
            Solution = SolutionSet.NoSolution;
            IsConsistent = false;
            Equations = equations;

            var _sol = new List<Vector<Fraction>> ();

            var (rows, columns) = equations.Rank;
            Fraction[,] mat = new Fraction[0, 0];

            if (columns == (rows + 1))
            {
                // Normal Equation
                mat = equations.ToArray ();
            }
            else if (rows >= columns)
            {
                // I assume we were given more rows than required (one or more)
                Fraction[,] mm = equations.ToArray ();
                mm.GetReducedRowEchelonForm ();
                var vMm = mm.AsMatrix ().ToVector (columns - 1);
                RemoveColumn (columns - 1, ref mm);
                // Check for potential Zero solutions, if not present, remove the unessecary rows please!

                for (int i = 0; i < mm.GetLength (0); i++)
                {
                    var vv = vMm[i];
                    bool rowIsZero = RowIsZeroOrEmpty (i, mm);

                    if (rowIsZero && vv != 0)
                    {
                        Solution = SolutionSet.NoSolution;
                        return;
                    }
                }

                // Remove unnecessary rows
                mat = equations.ToArray ();
                int rr = (rows + 1) - columns;

                for (int i = 0; i < rr; i++)
                {
                    // Remove them.
                    RemoveRow (rows - (i + 1), ref mat);
                }
                //mat = mat.AsMatrix().AppendColumn(0).ToArray();
                equations = mat.AsMatrix ();
            }
            else
            {
                // Adjust to make it in a normal form.
                // Not Consistent.

                int countR = equations.Rank.Item1;
                int count = equations.ToArray ().CountNonZeroRows ();

                // Oops...
                //mat = new Matrix<Fraction>(equations.ToArray()).ToArray();
                if (columns > rows)
                {
                    for (int i = 0; i < (columns - 1 - countR); i++)
                    {
                        // WARNING: WRITING TO equations here.
                        equations = equations.AppendRow (0).ToArray ();
                    }
                    mat = equations.ToArray ();
                    //mat = mat.AsMatrix().AppendColumn(0).ToArray();
                }
            }
            Equations = mat.AsMatrix ();

            var RREF = Equations.ToArray ();
            RREF.GetReducedRowEchelonForm ();
            var rrefMatrix = RREF.AsMatrix ();

            var REF = Equations.ToArray ();
            REF.GetRowEchelonForm ();
            var refMatrix = REF.AsMatrix ();

            IsConsistent = Equations.GetRank () == refMatrix.GetRank () ? true : false;
            var aug = Equations.ToVector (Equations.Rank.Item2 - 1);

            int c = 0;
            foreach (var item in aug)
            {
                if (item != 0)
                {
                    IsHomogenous = false;
                    break;
                }
                else
                {
                    if (c == (aug.Length - 1))
                    {
                        // Last Zero
                        IsHomogenous = true;
                        break;
                    }
                }
                c++;
            }

            // Full matrix with the rhs.
            var tempFull = equations.ToArray ();
            // Matrix without the rhs vector
            var temp = equations.ToArray ();
            // Remove rhs vector. 
            RemoveColumn (equations.Rank.Item2 - 1, ref temp);
            if (!(temp.GetDeterminant () == 0))
            {
                // Yay! Finite solution.
                var vector = rrefMatrix.ToVector (rrefMatrix.Rank.Item2 - 1);
                Solution = new SolutionSet (false, vector);
            }
            else
            {
                // Gotta deal with Infinite solutions.
                if (!IsHomogenous)
                {
                    // Could be Zero solutions or infinite
                    // How can we check?
                    tempFull.GetReducedRowEchelonForm ();
                    temp.GetReducedRowEchelonForm ();
                    bool ff = true;

                    for (int i = 0; i < temp.GetLength (0); i++)
                    {
                        if (RowIsZeroOrEmpty (i, temp))
                        {
                            var v = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);
                            if (v[i] != 0)
                            {
                                // No solutions.
                                Solution = SolutionSet.NoSolution;
                                ff = false;
                                break;
                            }
                        }
                    }

                    if (ff)
                    {
                        // Infinite solutions 
                        var free = temp.AsMatrix ().GetFreeVariables2 ();

                        // A temporary store for the solutions.
                        Fraction[,] sol = new Fraction[temp.GetLength (0), (free.Positions.Count + 1)];

                        var rhs = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);

                        for (int i = 0; i < temp.GetLength (0); i++)
                        {
                            // Current row we are handling.
                            var row = temp.GetRow (i);

                            // Index, indicating, the variable. e.g x1, x1, x2.
                            // NOTE: This is aa zero based index.
                            int? index = row.FindFirstNonZeroIndex ();

                            // Right hand side of the current row.
                            var rrhs = rhs[i];
                            bool flag = true;
                            if (index != null)
                            {
                                sol[(int)index, 0] = rrhs;
                            }
                            else
                            {
                                // Row and rhs is Zero (At least it should be, if all the logic is correct.)
                                flag = false;
                                Debug.Assert (RowIsZeroOrEmpty (i, temp) && (rrhs == 0));
                                continue;
                            }

                            // Zero index is for RHS, that's why we're starting from 1.
                            int ccCount = 1;
                            if (flag)
                            {
                                foreach (var item in free)
                                {
                                    var f = (-row[item]);
                                    sol[(int)index, ccCount] = f;
                                    ccCount++;
                                }
                            }
                        }

                        // To add free variables (of value: 1) in the right locations.
                        // Loop through the columns
                        // (sol.GetLength(1) - 1) -- this is because of the zero index is for rhs ONLY.
                        for (int i = 0; i < sol.GetLength (1) - 1; i++)
                        {
                            var pos = free[i];
                            // (i + 1) -- Zero index is for rhs ONLY.
                            sol[(pos), (i + 1)] = 1;
                        }
                        _sol = new List<Vector<Fraction>> (sol.AsMatrix ().MatrixToVectors ());
                        Solution = new SolutionSet (true, _sol);
                    }
                }
                else
                {
                    // Homogeneous
                    // Infinite solutions 
                    
                    var free = temp.AsMatrix ().GetFreeVariables2 ();

                    // A temporary store for the solutions.
                    Fraction[,] sol = new Fraction[temp.GetLength (0), (free.Positions.Count)];

                    var rhs = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);

                    for (int i = 0; i < temp.GetLength (0); i++)
                    {
                        // Current row we are handling.
                        var row = temp.GetRow (i);

                        // Index, indicating, the variable. e.g x1, x2, x3.
                        // NOTE: This is a zero based index (Obviously, we're programmers 😂).
                        int? index = row.FindFirstNonZeroIndex ();
                        bool flag = false;

                        // Right hand side of the current row.
                        //var rrhs = rhs[i];

                        if (index != null)
                        {
                            //sol[(int)index, 0] = rrhs;
                            flag = true;
                        }
                        else
                        {
                            // Row and rhs is Zero (At least it should be, if all the logic is correct 😁.)
                            // Debug.Assert(RowIsZeroOrEmpty(i, temp) && (rrhs == 0));
                            continue;
                        }

                        int ccCount = 0;
                        if (flag)
                        {
                            foreach (var item in free)
                            {
                                var f = (-row[item]);
                                sol[(int)index, ccCount] = f;
                                ccCount++;
                            }
                        }
                    }

                    // To add free variables (1) in the right locations.
                    // Loop through the columns
                    for (int i = 0; i < sol.GetLength (1); i++)
                    {
                        var pos = free[i];
                        sol[pos, i] = 1;
                    }
                    _sol.AddRange (sol.AsMatrix ().MatrixToVectors ());
                    Solution = new SolutionSet (true, _sol);
                }
            }
        }

        public Matrix<Fraction> Equations { get; }
        public SolutionSet Solution { get; }
        public bool IsHomogenous { get; }
        public bool IsConsistent { get; }

        public static SolutionSet Solve (Matrix<Fraction> equations)
        {
            return new LinearSystem (equations).Solution;
        }

        public readonly struct SolutionSet : IEnumerable<Vector<Fraction>>, IEquatable<SolutionSet>
        {
            public SolutionSet (bool infinite, params Vector<Fraction>[] vectors)
            {
                Set = new HashSet<Vector<Fraction>> (vectors);
                IsInfinite = infinite;
            }

            public SolutionSet (bool infinite, IEnumerable<Vector<Fraction>> vectors)
                : this (infinite, vectors.ToArray ())
            {
            }

            public static SolutionSet NoSolution = new SolutionSet ();

            public HashSet<Vector<Fraction>> Set { get; }
            public bool IsInfinite { get; }

            public bool Equals (SolutionSet other)
            {
                return Set.SetEquals (other.Set);
            }

            public IEnumerator<Vector<Fraction>> GetEnumerator ()
            {
                foreach (var solution in Set)
                {
                    yield return solution;
                }
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }

            // TODO: Make it look like a standard solution set; a linear combination with the free variables as constants.
            public override string ToString ()
            {
                return base.ToString ();
            }
        }

        public readonly struct FreeVariables : IEnumerable<int>
        {
            public FreeVariables (IEnumerable<int> free)
            {
                Positions = free.ToList ();
            }

            public List<int> Positions { get; }

            public int this[int index] => Positions[index];

            public IEnumerator<int> GetEnumerator() => Positions.GetEnumerator ();

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
        }
    }
}
=======
﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static MathAPI.Helpers.MatrixHelpers;
using static MathAPI.Helpers.LinearSystemHelpers;

namespace MathAPI.MathTypes
{
    public readonly struct LinearSystem
    {
        public LinearSystem (Matrix<Fraction> equations)
        {
            // Adjusted as the equations are reduced and solved.
            IsHomogenous = false;
            Solution = SolutionSet.NoSolution;
            IsConsistent = false;
            Equations = equations;

            var _sol = new List<Vector<Fraction>> ();

            var (rows, columns) = equations.Rank;
            Fraction[,] mat = new Fraction[0, 0];

            if (columns == (rows + 1))
            {
                // Normal Equation
                mat = equations.ToArray ();
            }
            else if (rows >= columns)
            {
                // I assume we were given more rows than required (one or more)
                Fraction[,] mm = equations.ToArray ();
                mm.GetReducedRowEchelonForm ();
                var vMm = mm.AsMatrix ().ToVector (columns - 1);
                RemoveColumn (columns - 1, ref mm);
                // Check for potential Zero solutions, if not present, remove the unessecary rows please!

                for (int i = 0; i < mm.GetLength (0); i++)
                {
                    var vv = vMm[i];
                    bool rowIsZero = RowIsZeroOrEmpty (i, mm);

                    if (rowIsZero && vv != 0)
                    {
                        Solution = SolutionSet.NoSolution;
                        return;
                    }
                }

                // Remove unnecessary rows
                mat = equations.ToArray ();
                int rr = (rows + 1) - columns;

                for (int i = 0; i < rr; i++)
                {
                    // Remove them.
                    RemoveRow (rows - (i + 1), ref mat);
                }
                //mat = mat.AsMatrix().AppendColumn(0).ToArray();
                equations = mat.AsMatrix ();
            }
            else
            {
                // Adjust to make it in a normal form.
                // Not Consistent.

                int countR = equations.Rank.Item1;
                int count = equations.ToArray ().CountNonZeroRows ();

                // Oops...
                //mat = new Matrix<Fraction>(equations.ToArray()).ToArray();
                if (columns > rows)
                {
                    for (int i = 0; i < (columns - 1 - countR); i++)
                    {
                        // WARNING: WRITING TO equations here.
                        equations = equations.AppendRow (0).ToArray ();
                    }
                    mat = equations.ToArray ();
                    //mat = mat.AsMatrix().AppendColumn(0).ToArray();
                }
            }
            Equations = mat.AsMatrix ();

            var RREF = Equations.ToArray ();
            RREF.GetReducedRowEchelonForm ();
            var rrefMatrix = RREF.AsMatrix ();

            var REF = Equations.ToArray ();
            REF.GetRowEchelonForm ();
            var refMatrix = REF.AsMatrix ();

            IsConsistent = Equations.GetRank () == refMatrix.GetRank () ? true : false;
            var aug = Equations.ToVector (Equations.Rank.Item2 - 1);

            int c = 0;
            foreach (var item in aug)
            {
                if (item != 0)
                {
                    IsHomogenous = false;
                    break;
                }
                else
                {
                    if (c == (aug.Length - 1))
                    {
                        // Last Zero
                        IsHomogenous = true;
                        break;
                    }
                }
                c++;  // Pun intended! (C++ in C#)
            }

            // Full matrix with the rhs.
            var tempFull = equations.ToArray ();
            // Matrix without the rhs vector
            var temp = equations.ToArray ();
            // Remove rhs vector. 
            RemoveColumn (equations.Rank.Item2 - 1, ref temp);
            if (!(temp.GetDeterminant () == 0))
            {
                // Yay! Finite solution.
                var vector = rrefMatrix.ToVector (rrefMatrix.Rank.Item2 - 1);
                Solution = new SolutionSet (false, vector);
            }
            else
            {
                // Gotta deal with Infinite solutions.
                if (!IsHomogenous)
                {
                    // Could be Zero solutions or infinite
                    // How can we check?
                    tempFull.GetReducedRowEchelonForm ();
                    temp.GetReducedRowEchelonForm ();
                    bool ff = true;

                    for (int i = 0; i < temp.GetLength (0); i++)
                    {
                        if (RowIsZeroOrEmpty (i, temp))
                        {
                            var v = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);
                            if (v[i] != 0)
                            {
                                // No solutions.
                                Solution = SolutionSet.NoSolution;
                                ff = false;
                                break;
                            }
                        }
                    }

                    if (ff)
                    {
                        // Infinite solutions 
                        var free = temp.AsMatrix ().GetFreeVariables2 ();

                        // A temporary store for the solutions.
                        Fraction[,] sol = new Fraction[temp.GetLength (0), (free.Positions.Count + 1)];

                        var rhs = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);

                        for (int i = 0; i < temp.GetLength (0); i++)
                        {
                            // Current row we are handling.
                            var row = temp.GetRow (i);

                            // Index, indicating, the variable. e.g x1, x1, x2.
                            // NOTE: This is aa zero based index.
                            // TODO: I should use C# 8 features here that ?? thingy for this nullable guy
                            int? index = row.FindFirstNonZeroIndex ();

                            // Right hand side of the current row.
                            var rrhs = rhs[i];
                            bool flag = true;
                            if (index != null)
                            {
                                sol[(int)index, 0] = rrhs;
                            }
                            else
                            {
                                // Row and rhs is Zero (At least it should be, if all the logic is correct.)
                                flag = false;
                                Debug.Assert (RowIsZeroOrEmpty (i, temp) && (rrhs == 0));
                                continue;
                            }

                            // Zero index is for RHS, that's why we're starting from 1.
                            int ccCount = 1;
                            if (flag)
                            {
                                foreach (var item in free)
                                {
                                    var f = (-row[item]);
                                    sol[(int)index, ccCount] = f;
                                    ccCount++;
                                }
                            }
                        }

                        // To add free variables (of value: 1) in the right locations.
                        // Loop through the columns
                        // (sol.GetLength(1) - 1) -- this is because of the zero index is for rhs ONLY.
                        for (int i = 0; i < sol.GetLength (1) - 1; i++)
                        {
                            var pos = free[i];
                            // (i + 1) -- Zero index is for rhs ONLY.
                            sol[(pos), (i + 1)] = 1;
                        }
                        _sol = new List<Vector<Fraction>> (sol.AsMatrix ().MatrixToVectors ());
                        Solution = new SolutionSet (true, _sol);
                    }
                }
                else
                {
                    // Homogeneous
                    // Infinite solutions 
                    
                    var free = temp.AsMatrix ().GetFreeVariables2 ();

                    // A temporary store for the solutions.
                    Fraction[,] sol = new Fraction[temp.GetLength (0), (free.Positions.Count)];

                    var rhs = tempFull.AsMatrix ().ToVector (tempFull.GetLength (1) - 1);

                    for (int i = 0; i < temp.GetLength (0); i++)
                    {
                        // Current row we are handling.
                        var row = temp.GetRow (i);

                        // Index, indicating, the variable. e.g x1, x2, x3.
                        // NOTE: This is a zero based index (Obviously, we're programmers 😂).
                        int? index = row.FindFirstNonZeroIndex ();
                        bool flag = false;

                        // Right hand side of the current row.
                        //var rrhs = rhs[i];

                        if (index != null)
                        {
                            //sol[(int)index, 0] = rrhs;
                            flag = true;
                        }
                        else
                        {
                            // Row and rhs is Zero (At least it should be, if all the logic is correct 😁.)
                            // Debug.Assert(RowIsZeroOrEmpty(i, temp) && (rrhs == 0));
                            continue;
                        }

                        int ccCount = 0;
                        if (flag)
                        {
                            foreach (var item in free)
                            {
                                var f = (-row[item]);
                                sol[(int)index, ccCount] = f;
                                ccCount++;
                            }
                        }
                    }

                    // To add free variables (1) in the right locations.
                    // Loop through the columns
                    for (int i = 0; i < sol.GetLength (1); i++)
                    {
                        var pos = free[i];
                        sol[pos, i] = 1;
                    }
                    _sol.AddRange (sol.AsMatrix ().MatrixToVectors ());
                    Solution = new SolutionSet (true, _sol);
                }
            }
        }

        public Matrix<Fraction> Equations { get; }
        public SolutionSet Solution { get; }
        public bool IsHomogenous { get; }
        public bool IsConsistent { get; }

        public static SolutionSet Solve (Matrix<Fraction> equations)
        {
            return new LinearSystem (equations).Solution;
        }

        public readonly struct SolutionSet : IEnumerable<Vector<Fraction>>, IEquatable<SolutionSet>
        {
            public SolutionSet (bool infinite, params Vector<Fraction>[] vectors)
            {
                Set = new HashSet<Vector<Fraction>> (vectors);
                IsInfinite = infinite;
            }

            public SolutionSet (bool infinite, IEnumerable<Vector<Fraction>> vectors)
                : this (infinite, vectors.ToArray ())
            {
            }

            public static SolutionSet NoSolution = new SolutionSet ();

            public HashSet<Vector<Fraction>> Set { get; }
            public bool IsInfinite { get; }

            public bool Equals (SolutionSet other)
            {
                return Set.SetEquals (other.Set);
            }

            public IEnumerator<Vector<Fraction>> GetEnumerator ()
            {
                foreach (var solution in Set)
                {
                    yield return solution;
                }
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
                return GetEnumerator ();
            }

            // TODO: Make it look like a standard solution set; a linear combination with the free variables as constants.
            public override string ToString ()
            {
                return base.ToString ();
            }
        }

        public readonly struct FreeVariables : IEnumerable<int>
        {
            public FreeVariables (IEnumerable<int> free)
            {
                Positions = free.ToList ();
            }

            public List<int> Positions { get; }

            public int this[int index] => Positions[index];

            public IEnumerator<int> GetEnumerator() => Positions.GetEnumerator ();

            IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
        }
    }
}
>>>>>>> 024dc94e61b8feebc7a2946485fbcd0de4922671
