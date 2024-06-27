export interface User {
  email: string;
  displayName: string;
  token: string;
}
export interface UserRegister {
  email: string;
  password: string;
  confirmPassword: string;
  displayName: string;
  softwareDistributionGroup: string;
}
