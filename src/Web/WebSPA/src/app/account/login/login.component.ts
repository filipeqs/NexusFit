import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  returnUrl: string;
  loginForm: FormGroup;
  errors: string[] = [];

  constructor(
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams.returnUrl || '';
    this.createLoginForm();
  }

  createLoginForm() {
    this.loginForm = new FormGroup({
      email: new FormControl('', [
        Validators.required,
        Validators.pattern(
          '^\\w+[\\w-\\.]*\\@\\w+((-\\w+)|(\\w*))\\.[a-z]{2,3}$'
        ),
      ]),
      password: new FormControl('', Validators.required),
    });
  }

  onSubmit() {
    this.clearErrors();
    this.login();
  }

  login() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        this.router.navigateByUrl(this.returnUrl);
      },
      error: (error) => {
        console.log(error);
        this.errors.push('Invalid email or password');
      },
    });
  }

  clearErrors() {
    this.errors = [];
  }
}
