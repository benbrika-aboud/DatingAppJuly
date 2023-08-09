import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  registerForm: FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;

  // model:any= {};
  // @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  
  constructor(private accountService: AccountService, 
              private toastr: ToastrService, 
              private fb: FormBuilder,
              private router: Router) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }

  // initializeForm() {
  //   this.registerForm = new FormGroup({
  //     username: new FormControl('', Validators.required),
  //     password: new FormControl('',[Validators.required, Validators.maxLength(8),Validators.minLength(4)]),
  //     confirmPassword: new FormControl('', [Validators.required, this.matchValue('password')])
  //   })

  //   this.registerForm.controls['password'].valueChanges.subscribe({
  //     next: ()=> this.registerForm.controls['confirmPassword'].updateValueAndValidity()
  //   })
  // }

  // on va refaire la methode en utilisant les FormBuilder
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['',[Validators.required, Validators.maxLength(8),Validators.minLength(4)]],
      confirmPassword: ['', [Validators.required, this.matchValue('password')]]
    })

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: ()=> this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  matchValue(matchTo: string) {
    return (control: AbstractControl) => {
      if(control.value === control.parent?.get(matchTo)?.value) return null 
      else return { notMatching: true }
    }
  }

  register() {
    
    
    const dob = this.getDateOnly(this.registerForm?.controls['dateOfBirth'].value);
    
    //normalement const value va avoir la valeur de l'objet this.registerForm.value mais en remplacant
    // le champ dateOfBirth par la valeur du const dob
    const value = {...this.registerForm.value,dateOfBirth: dob};
    
    // console.log(value);
    this.accountService.register(value).subscribe({
      next: response => {
        this.cancel();
        this.router.navigateByUrl("/members");
      },
      error: error=> {
        this.validationErrors = error;        
      }
    })
  }

  private getDateOnly(dob: string | undefined) {
    if(!dob) return;

    let theDob = new Date(dob);

    return new Date(theDob.setMinutes(theDob.getMinutes()-theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }

  cancel() {
    this.cancelRegister.emit(false);
    
  }

}
