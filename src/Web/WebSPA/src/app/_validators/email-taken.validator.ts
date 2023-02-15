import { AsyncValidatorFn } from '@angular/forms';
//import { map, of, switchMap, timer } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { of, timer } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';

export function validateEmailNotTaken(
  accountService: AccountService
): AsyncValidatorFn {
  return (control) => {
    return timer(500).pipe(
      switchMap(() => {
        if (!control.value) {
          return of(null);
        }

        return accountService.checkEmailExists(control.value).pipe(
          map((res) => {
            return res ? { emailExists: true } : null;
          })
        );
      })
    );
  };
}
