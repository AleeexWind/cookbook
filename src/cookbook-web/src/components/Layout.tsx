import { Link, NavLink } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useAuth } from '../auth'

export function Layout({ children }: { children: ReactNode }) {
  const { token, userName, logout } = useAuth()

  return (
    <div className="app-shell">
      <header className="topbar">
        <Link to="/" className="brand">
          Книга рецептов
        </Link>
        <nav>
          <NavLink to="/">Рецепты</NavLink>
          <NavLink to="/menu">План меню</NavLink>
          {token ? (
            <>
              <span className="user">{userName}</span>
              <button type="button" className="linkish" onClick={logout}>
                Выйти
              </button>
            </>
          ) : (
            <NavLink to="/login">Войти</NavLink>
          )}
        </nav>
      </header>
      {children}
    </div>
  )
}
