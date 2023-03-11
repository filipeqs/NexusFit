import { Component, OnInit } from '@angular/core';
import {
    AbstractControlOptions,
    FormBuilder,
    FormControl,
    FormGroup,
    Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ConfirmedValidator } from 'src/app/_validators/confirmed.validator';
import { validateEmailNotTaken } from 'src/app/_validators/email-taken.validator';
import { AccountService } from '../account.service';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
    registerForm: FormGroup;
    errors: string[] = [];

    constructor(
        private accountService: AccountService,
        private router: Router,
        private fb: FormBuilder,
    ) {}

    ngOnInit(): void {
        this.createRegisterForm();
    }

    createRegisterForm() {
        this.registerForm = this.fb.group(
            {
                email: [
                    '',
                    [
                        Validators.required,
                        Validators.pattern('^\\w+[\\w-\\.]*\\@\\w+((-\\w+)|(\\w*))\\.[a-z]{2,3}$'),
                    ],
                    [validateEmailNotTaken(this.accountService)],
                ],
                password: ['', Validators.required],
                confirmPassword: ['', Validators.required],
            },
            {
                validator: ConfirmedValidator('password', 'confirmPassword'),
            } as AbstractControlOptions,
        );
    }

    onSubmit() {
        this.clearErrors();
        this.register();
    }

    register() {
        this.accountService.register(this.registerForm.value).subscribe({
            next: () => {
                this.router.navigateByUrl('/');
            },
            error: (error) => {
                console.log(error);
                this.errors = error;
            },
        });
    }

    clearErrors() {
        this.errors = [];
    }
}
