import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  model:any= {};
  // @Input() usersFromHomeComponent: any;
  @Output() cancelRegister = new EventEmitter();
  
  constructor(private accountService: AccountService, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  register() {
    this.accountService.register(this.model).subscribe({
      next: response => {
        this.cancel();
      },
      error: error=> {
        this.toastr.error(error.error);
      }
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
    
  }

}
