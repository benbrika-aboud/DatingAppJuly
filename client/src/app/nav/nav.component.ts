import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
  // loggedIn = false;
  // currentUser$: Observable<User| null> = of(null)

  constructor(public accountService: AccountService) { }

  ngOnInit(): void {
    // this.getCurrentUser();

    // this.currentUser$ = this.accountService.currentUser$;
  }

  // getCurrentUser() {
  //   this.accountService.currentUser$.subscribe({
  //     next: user => {
  //       this.loggedIn = !!user;
  //       //ca veut dire si on a un user cela retourne true. si user est null ca retourne false
  //     },
  //     error: error => console.log(error)
  //   });
  // }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response=> {
        console.log(response);
        // this.loggedIn= true;
      },
      error: error=> {
        console.log(error);
      }
    })
    
  }

  logout() {
    this.accountService.logout();
    // this.loggedIn = false;
  }

}
