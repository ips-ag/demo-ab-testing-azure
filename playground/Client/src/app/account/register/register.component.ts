import { Component } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { AccountService } from '../account.service';
import { LoadingService } from 'src/app/core/services/loading.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import {
  EXPERIMENTAL_SOFTWARE_VERSION,
  EXPERIMENTAL_USER_GROUP,
  STABLE_SOFTWARE_VERSION,
  STABLE_USER_GROUP,
} from '../constants';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
  registerForm: FormGroup;
  loading: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private loadingService: LoadingService,
    private router: Router,
    private toastr: ToastrService
  ) {
    this.registerForm = this.formBuilder.group(
      {
        email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', [Validators.required]],
        displayName: ['', [Validators.required]],
        softwareDistributionGroup: [false],
      },
      {
        validator: this.mustMatch('password', 'confirmPassword'),
      }
    );
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.loadingService.loading();
      this.registerForm.value.softwareDistributionGroup = this.registerForm
        .value?.softwareDistributionGroup
        ? EXPERIMENTAL_USER_GROUP
        : STABLE_USER_GROUP;

      if (
        !/(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}/.test(
          this.registerForm.value.password
        )
      ) {
        this.loadingService.idle();
        this.toastr.error(
          'Password must contain at least one digit, one lowercase letter, one uppercase letter, one special character, and be at least 8 characters long'
        );
        return;
      }

      this.accountService.register(this.registerForm.value).subscribe({
        next: (user) => {
          // Handle successful registration
          this.loadingService.idle();
          this.toastr.success('Registration Successful');
          // Redirect to the login page or perform other actions
          this.router.navigate(['/account/login']);
        },
        error: (error) => {
          // Handle registration error
          this.loadingService.idle();

          // Check if the error message is 'Email is already in use'
          if (error.message === 'Email is already in use') {
            // Show a Toastr error message
            this.toastr.error('Email is already in use');
          } else {
            // Handle other errors here
            this.toastr.error('An error occurred');
          }
        },
      });
    }
  }

  // Custom validator to ensure password and confirmPassword match
  mustMatch(
    controlName: string,
    matchingControlName: string
  ): (formGroup: FormGroup) => null | { mustMatch: boolean } {
    return (formGroup: FormGroup): null | { mustMatch: boolean } => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];

      if (control.value !== matchingControl.value) {
        matchingControl.setErrors({ mustMatch: true });
      } else {
        matchingControl.setErrors(null);
      }

      return null;
    };
  }
}
