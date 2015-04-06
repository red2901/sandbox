;;; set some colours etc ...
;;; this highlights the parenthese
(show-paren-mode 1)

;;; this set's the cursor color to red
(set-cursor-color "White")

;;; Hightlight the current region 
(transient-mark-mode t) 
;;; display the time
(display-time)

;;; always use font-lock-mode
(setq global-font-lock-mode t)
(setq font-lock-maximum-decoration t)

;; set the face
(set-foreground-color  "White" ) 
(set-background-color  "Black" )
(set-face-foreground 'region "Green" )
(set-face-background 'region "ForestGreen" )
;; (set-face-foreground 'modeline "Green" )
;; (set-face-background 'modeline "DarkGreen" ) 
(set-background-color  "Black" )
(set-cursor-color  "White" )

;; (set-default-font "-Adobe-Courier-Medium-R-Normal--18-180-75-75-M-110-ISO8859-1")

;;
;; spaces, not tabs ! 
(setq-default indent-tabs-mode nil)

;; turn off blinking cursor mode
(if (fboundp 'blink-cursor-mode) (blink-cursor-mode 0))

;; turn the font lock modes on by default 
;; hooks to major modes
(add-hook 'dired-mode-hook 'turn-on-font-lock) 
(add-hook 'emacs-lisp-mode-hook 'turn-on-font-lock)

;; emacs library path
(add-to-list 'load-path "~/emacs-lisp/")

;;
;; setting up some major modes
;;
;; i think sql mode is already loaded so I'm going to list some of it's files
(load "sql")
(autoload 'sql-mode "sql") 
(add-hook 'sql-mode-hook 'turn-on-font-lock)
(setq auto-mode-alist (cons '("\.ddl$" . sql-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.sp$" . sql-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.dml$" . sql-mode) auto-mode-alist)) 
(setq auto-mode-alist (cons '("\.sql$" . sql-mode) auto-mode-alist))

;;
;; this is c-mode
(add-hook 'c-mode-hook '(lambda () (c-set-style "ellemtel")))
(add-hook 'c++-mode-hook '(lambda () (c-set-style "ellemtel"))) 
(setq auto-mode-alist (cons '("\.cpp$" . c++-mode) auto-mode-alist))
(setq auto-mode-alist (cons '("\.hpp$" . c++-mode) auto-mode-alist))

(add-hook 'c++-mode-hook 'turn-on-font-lock)
(add-hook 'c-mode-hook 'turn-on-font-lock) 

;; 
;; this is csharp-mode
(autoload 'csharp-mode "csharp-mode" "Major mode for editing C# code." t)
(setq auto-mode-alist
   (append '(("\\.cs$" . csharp-mode)) auto-mode-alist))

;;
;; this is python-mode
(setq auto-mode-alist (cons '("\.pyw$" . python-mode) auto-mode-alist)) 
(setq auto-mode-alist (cons '("\.py$" . python-mode) auto-mode-alist))


;;
;; global set keys
;; C-x C-y for shell
(global-set-key "\C-x\C-y" 'shell)
(global-set-key "\C-x\C-r" 'replace-string)
;; C-x C-t
;;
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


