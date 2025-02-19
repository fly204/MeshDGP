/*
   ARPACK++ v1.2 2/20/2000
   c++ interface to ARPACK code.

   MODULE ARDGNSym.h.
   Arpack++ class ARluNonSymGenEig definition
   (dense matrix version).

   ARPACK Authors
      Richard Lehoucq
      Danny Sorensen
      Chao Yang
      Dept. of Computational & Applied Mathematics
      Rice University
      Houston, Texas
*/

#ifndef ARDGNSYM_H
#define ARDGNSYM_H

#include <cstddef>

#include "arch.h"
#include "ardnsmat.h"
#include "ardnspen.h"
#include "argnsym.h"


template<class ARFLOAT>
class ARluNonSymGenEig:
  public virtual ARNonSymGenEig<ARFLOAT, ARdsNonSymPencil<ARFLOAT, ARFLOAT>,
                                ARdsNonSymPencil<ARFLOAT, ARFLOAT> > {

 private:

 // a) Data structure used to store matrices.

  ARdsNonSymPencil<ARFLOAT, ARFLOAT> Pencil;

 // b) Protected functions:

  virtual void Copy(const ARluNonSymGenEig& other);
  // Makes a deep copy of "other" over "this" object.
  // Old values are not deleted (this function is to be used
  // by the copy constructor and the assignment operator only).


 public:

 // c) Public functions:

 // c.1) Functions that allow changes in problem parameters.

  virtual void ChangeShift(ARFLOAT sigmaRp, ARFLOAT sigmaIp = 0.0);

  virtual void SetRegularMode();

  virtual void SetShiftInvertMode(ARFLOAT sigmap);

  virtual void SetComplexShiftMode(char partp, ARFLOAT sigmaRp,ARFLOAT sigmaIp);

 // c.2) Constructors and destructor.

  ARluNonSymGenEig() { }
  // Short constructor.

  ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                   ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, char* whichp = "LM",
                   int ncvp = 0, ARFLOAT tolp = 0.0, int maxitp = 0,
                   ARFLOAT* residp = NULL, bool ishiftp = true);
  // Long constructor (regular mode).

  ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                   ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, ARFLOAT sigma,
                   char* whichp = "LM", int ncvp = 0,
                   ARFLOAT tolp = 0.0, int maxitp = 0,
                   ARFLOAT* residp = NULL, bool ishiftp = true);
  // Long constructor (real shift and invert mode).

  ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                   ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, char partp,
                   ARFLOAT sigmaRp, ARFLOAT sigmaIp, char* whichp = "LM",
                   int ncvp = 0, ARFLOAT tolp = 0.0, int maxitp = 0,
                   ARFLOAT* residp = NULL, bool ishiftp = true);
  // Long constructor (complex shift and invert mode).

  ARluNonSymGenEig(const ARluNonSymGenEig& other) { Copy(other); }
  // Copy constructor.

  virtual ~ARluNonSymGenEig() { }
  // Destructor.

 // d) Operators.

  ARluNonSymGenEig& operator=(const ARluNonSymGenEig& other);
  // Assignment operator.

}; // class ARluNonSymGenEig.


// ------------------------------------------------------------------------ //
// ARluNonSymGenEig member functions definition.                            //
// ------------------------------------------------------------------------ //


template<class ARFLOAT>
inline void ARluNonSymGenEig<ARFLOAT>::
Copy(const ARluNonSymGenEig<ARFLOAT>& other)
{

  ARNonSymGenEig<ARFLOAT, ARdsNonSymPencil<ARFLOAT, ARFLOAT>,
                 ARdsNonSymPencil<ARFLOAT, ARFLOAT> >:: Copy(other);
  Pencil = other.Pencil;
  this->objOP  = &Pencil;
  this->objB   = &Pencil;
  this->objA   = &Pencil;

} // Copy.


template<class ARFLOAT>
inline void ARluNonSymGenEig<ARFLOAT>::
ChangeShift(ARFLOAT sigmaRp, ARFLOAT sigmaIp)
{

  if (sigmaIp == 0.0) {
    this->objOP->FactorAsB(sigmaRp);
  }
  else {
    this->objOP->FactorAsB(sigmaRp, sigmaIp, this->part);
  }
  ARrcNonSymGenEig<ARFLOAT>::ChangeShift(sigmaRp, sigmaIp);

} // ChangeShift.


template<class ARFLOAT>
inline void ARluNonSymGenEig<ARFLOAT>::SetRegularMode()
{

  ARStdEig<ARFLOAT, ARFLOAT, ARdsNonSymPencil<ARFLOAT, ARFLOAT> >::
    SetRegularMode(&Pencil, &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvBAv);

} // SetRegularMode.


template<class ARFLOAT>
inline void ARluNonSymGenEig<ARFLOAT>::SetShiftInvertMode(ARFLOAT sigmap)
{

  ARNonSymGenEig<ARFLOAT, ARdsNonSymPencil<ARFLOAT, ARFLOAT>,
                 ARdsNonSymPencil<ARFLOAT, ARFLOAT> >::
    SetShiftInvertMode(sigmap, &Pencil,
                       &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvAsBv);

} // SetShiftInvertMode.


template<class ARFLOAT>
inline void ARluNonSymGenEig<ARFLOAT>::
SetComplexShiftMode(char partp, ARFLOAT sigmaRp, ARFLOAT sigmaIp)
{

  ARNonSymGenEig<ARFLOAT, ARdsNonSymPencil<ARFLOAT, ARFLOAT>,
                 ARdsNonSymPencil<ARFLOAT, ARFLOAT> >::
    SetComplexShiftMode(partp, sigmaRp, sigmaIp, &Pencil,
                        &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvAsBv,
                        &Pencil, &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultAv);

} // SetComplexShiftMode.


template<class ARFLOAT>
inline ARluNonSymGenEig<ARFLOAT>::
ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                 ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, char* whichp, int ncvp,
                 ARFLOAT tolp, int maxitp, ARFLOAT* residp, bool ishiftp)

{

  Pencil.DefineMatrices(A, B);
  this->NoShift();
  DefineParameters(A.ncols(), nevp, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvBAv, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultBv, whichp,
                   ncvp, tolp, maxitp, residp, ishiftp);

} // Long constructor (regular mode).


template<class ARFLOAT>
inline ARluNonSymGenEig<ARFLOAT>::
ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                 ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, ARFLOAT sigmap,
                 char* whichp, int ncvp, ARFLOAT tolp,
                 int maxitp, ARFLOAT* residp, bool ishiftp)

{

  Pencil.DefineMatrices(A, B);
  DefineParameters(A.ncols(), nevp, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvAsBv, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultBv, whichp,
                   ncvp, tolp, maxitp, residp, ishiftp);
  SetShiftInvertMode(sigmap);

} // Long constructor (real shift and invert mode).


template<class ARFLOAT>
inline ARluNonSymGenEig<ARFLOAT>::
ARluNonSymGenEig(int nevp, ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& A,
                 ARdsNonSymMatrix<ARFLOAT, ARFLOAT>& B, 
                 char partp, ARFLOAT sigmaRp, ARFLOAT sigmaIp, char* whichp, 
                 int ncvp, ARFLOAT tolp, int maxitp, ARFLOAT* residp, 
                 bool ishiftp)

{

  Pencil.DefineMatrices(A, B);
  DefineParameters(A.ncols(), nevp, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultInvAsBv, &Pencil,
                   &ARdsNonSymPencil<ARFLOAT, ARFLOAT>::MultBv, whichp,
                   ncvp, tolp, maxitp, residp, ishiftp);
  SetComplexShiftMode(partp, sigmaRp, sigmaIp);

} // Long constructor (complex shift and invert mode).


template<class ARFLOAT>
ARluNonSymGenEig<ARFLOAT>& ARluNonSymGenEig<ARFLOAT>::
operator=(const ARluNonSymGenEig<ARFLOAT>& other)
{

  if (this != &other) { // Stroustrup suggestion.
    this->ClearMem();
    Copy(other);
  }
  return *this;

} // operator=.


#endif // ARDGNSYM_H
