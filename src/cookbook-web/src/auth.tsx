import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import type { AuthResult } from './api'
import * as api from './api'

type AuthState = {
  token: string | null
  userName: string | null
  login: (userName: string, password: string) => Promise<void>
  register: (userName: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthState | null>(null)
const STORAGE_KEY = 'cookbook.auth'

function loadStored(): AuthResult | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as AuthResult) : null
  } catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const stored = loadStored()
  const [token, setToken] = useState<string | null>(stored?.accessToken ?? null)
  const [userName, setUserName] = useState<string | null>(stored?.userName ?? null)

  const value = useMemo<AuthState>(
    () => ({
      token,
      userName,
      async login(name, password) {
        const result = await api.login(name, password)
        localStorage.setItem(STORAGE_KEY, JSON.stringify(result))
        setToken(result.accessToken)
        setUserName(result.userName)
      },
      async register(name, password) {
        const result = await api.register(name, password)
        localStorage.setItem(STORAGE_KEY, JSON.stringify(result))
        setToken(result.accessToken)
        setUserName(result.userName)
      },
      logout() {
        localStorage.removeItem(STORAGE_KEY)
        setToken(null)
        setUserName(null)
      },
    }),
    [token, userName],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthState {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('AuthProvider is required')
  return ctx
}
