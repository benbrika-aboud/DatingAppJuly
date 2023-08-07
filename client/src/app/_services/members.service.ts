import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MembersService {

  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) { }

  getmembers() {
    if(this.members.length > 0) return of(this.members);

    return this.http.get<Member[]>(this.baseUrl+'Users').pipe(
      map(mbs => {
        this.members = mbs;
        return mbs;
      })
    );
  }

  getMember(username:string) {
    const member = this.members.find(x=> x.userName == username);
    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl+'Users/'+username)
  }

  getHttpOptions() {
    const userString = localStorage.getItem('user')

    if(!userString) return;

    const user = JSON.parse(userString);
    return {
      headers: new HttpHeaders({
        Authorization : 'Bearer '+user.token
      })
    }
  }

  updateMember(member:Member) {
    return this.http.put(this.baseUrl+'users',member).pipe(
      map(()=> {
        const index = this.members.indexOf(member);
        //du coup lecriture en bas veut dire quon va affecter a this.members[index]
        // un objet qui va etre rempli de ...this.members[index] puis qui va etre modifier avec les valeurs
        // recuperees de   ...member
        this.members[index] = {...this.members[index],...member};
      })
    );

  }

  setMainPhoto(photoId:number) {
    return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId,{});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);
  }
}
